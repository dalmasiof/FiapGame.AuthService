using DTOs;
using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace Auth.Application.Test;

public sealed class RsaTokenServiceTests
{
    [Fact]
    public void GerarToken_AssinaComRs256EValidaComChavePublica()
    {
        using var rsa = RSA.Create(2048);
        using var keyProvider = new RsaKeyProvider(
            rsa.ExportPkcs8PrivateKeyPem());

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "FiapGamesApi",
                ["Jwt:Audience"] = "FiapGamesClients"
            })
            .Build();

        var service = new TokenService(configuration, keyProvider);
        var login = new LerLoginDTO(
            1,
            "user",
            "user@fiapgames.test",
            "hash",
            "Ativo",
            TIPO_USUARIO.Usuario);

        var encodedToken = service.GerarToken(login);
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(encodedToken);

        Assert.Equal(SecurityAlgorithms.RsaSha256, token.Header.Alg);
        Assert.Equal(keyProvider.ValidationKey.KeyId, token.Header.Kid);

        handler.ValidateToken(encodedToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = keyProvider.ValidationKey,
            ValidateIssuer = true,
            ValidIssuer = "FiapGamesApi",
            ValidateAudience = true,
            ValidAudience = "FiapGamesClients",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out _);
    }

    [Fact]
    public void GetPublicJwk_RetornaSomenteParametrosPublicos()
    {
        using var rsa = RSA.Create(2048);
        using var keyProvider = new RsaKeyProvider(
            rsa.ExportPkcs8PrivateKeyPem());

        var jwk = keyProvider.GetPublicJwk();

        Assert.Equal("RSA", jwk.Kty);
        Assert.Equal("sig", jwk.Use);
        Assert.Equal(SecurityAlgorithms.RsaSha256, jwk.Alg);
        Assert.False(string.IsNullOrWhiteSpace(jwk.Kid));
        Assert.False(string.IsNullOrWhiteSpace(jwk.N));
        Assert.False(string.IsNullOrWhiteSpace(jwk.E));
    }
}
