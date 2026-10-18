using System.Net;
using System.Text.Json;
namespace Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
                if (context.Response.StatusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.Forbidden)
                    throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(
                "Request failed with {ExceptionType} for {Method} {Path}. TraceId: {TraceId}",
                ex.GetType().Name,
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            context.Response.ContentType = "application/json";

            var objErro = ex switch
            {
                UnauthorizedAccessException => new ErroResponseDto("Acesso não autorizado", HttpStatusCode.Unauthorized),

                ArgumentException => new ErroResponseDto($"Requisição inválida: {ex.Message}", HttpStatusCode.BadRequest),

                _ => new ErroResponseDto("Erro interno do servidor", HttpStatusCode.InternalServerError)
            };

            context.Response.StatusCode = (int)objErro.Status;

            await context.Response.WriteAsync(JsonSerializer.Serialize(objErro));
        }
        public record ErroResponseDto(string Erro, HttpStatusCode Status);
    }
}
