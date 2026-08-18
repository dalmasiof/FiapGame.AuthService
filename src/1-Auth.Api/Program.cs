using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Context;
using Interfaces;
using Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Middleware;
using Prometheus;
using Repository;
using Services;
using System.Net;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

if (!builder.Environment.IsDevelopment())
{
    var keyVaultUriValue = Environment.GetEnvironmentVariable("KeyVaultUri");
    if (!Uri.TryCreate(keyVaultUriValue, UriKind.Absolute, out var keyVaultUri) ||
        keyVaultUri.Scheme != Uri.UriSchemeHttps)
    {
        throw new InvalidOperationException(
            "Environment variable KeyVaultUri must contain a valid HTTPS URI.");
    }

    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential());
}

// Add services to the container.

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDataProtection().UseEphemeralDataProtectionProvider();
}
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var rabbitHost = builder.Configuration["RabbitMq:HostName"] ?? "localhost";
var rabbitPort = int.TryParse(builder.Configuration["RabbitMq:Port"], out var parsedPort) ? parsedPort : 5672;
var rabbitUser = builder.Configuration["RabbitMq:UserName"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMq:Password"] ?? "guest";

var rabbitConnectionFactory = new ConnectionFactory
{
    HostName = rabbitHost,
    Port = rabbitPort,
    UserName = rabbitUser,
    Password = rabbitPass,
    AutomaticRecoveryEnabled = true,
    NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
};
var rabbitHealthConnection = new Lazy<Task<IConnection>>(
    () => rabbitConnectionFactory.CreateConnectionAsync());

builder.Services.AddSingleton<IConnectionFactory>(rabbitConnectionFactory);

builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

var connectionString = builder.Configuration.GetConnectionString("AuthConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Connection string AuthConnection is required.");
}

var privateKeyPem = builder.Configuration["Jwt:PrivateKey"];
var privateKeyFile = builder.Configuration["Jwt:PrivateKeyFile"];
if (string.IsNullOrWhiteSpace(privateKeyPem) && !string.IsNullOrWhiteSpace(privateKeyFile))
{
    privateKeyPem = await File.ReadAllTextAsync(privateKeyFile);
}

var rsaKeyProvider = new RsaKeyProvider(privateKeyPem ?? string.Empty);
builder.Services.AddSingleton<IRsaKeyProvider>(_ => rsaKeyProvider);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Em prod, mude para true
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = rsaKeyProvider.ValidationKey,
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Remove o tempo de tolerância padrão do .NET
    };
});

builder.Services.AddAuthorization();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var erro = new
        {
            erro = "Requisição inválida",
            status = 400,
            detalhes = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value.Errors.Select(e => e.ErrorMessage)
                )
        };

        return new BadRequestObjectResult(erro);
    };
});

builder.Services.AddDbContext<AuthContext>(opts =>
    opts
    .UseLazyLoadingProxies()
    .UseSqlServer(connectionString));

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
    .AddSqlServer(connectionString, name: "sqlserver", tags: ["ready"])
    .AddRabbitMQ(
        _ => rabbitHealthConnection.Value,
        name: "rabbitmq",
        tags: ["ready"]);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var erros = context.ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .SelectMany(x => x.Value.Errors
                .Select(e => $"{x.Key}: {e.ErrorMessage}"))
                .ToList();

            return new BadRequestObjectResult(new
            {
                erro = "Requisição inválida",
                status = (int)HttpStatusCode.BadRequest,
                detalhes = erros
            });
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (args.Contains("--migrate", StringComparer.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthContext>();

    await context.Database.MigrateAsync();
    await DbInitializer.SeedAsync(context);

    Console.WriteLine("Auth database migrations and seeds applied successfully.");
    return;
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpMetrics();


app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/health"),
    branch => branch.UseHttpsRedirection());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapMetrics("/metrics");
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();
