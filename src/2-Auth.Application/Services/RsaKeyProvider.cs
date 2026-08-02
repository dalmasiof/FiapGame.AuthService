using Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Services;

public sealed class RsaKeyProvider : IRsaKeyProvider, IDisposable
{
    private readonly RSA _rsa;

    public SigningCredentials SigningCredentials { get; }
    public SecurityKey ValidationKey { get; }

    public RsaKeyProvider(string privateKeyPem)
    {
        if (string.IsNullOrWhiteSpace(privateKeyPem))
        {
            throw new InvalidOperationException(
                "Configuration key Jwt:PrivateKey is required.");
        }

        _rsa = RSA.Create();

        try
        {
            _rsa.ImportFromPem(NormalizePem(privateKeyPem));
        }
        catch (Exception ex) when (
            ex is ArgumentException or CryptographicException)
        {
            _rsa.Dispose();
            throw new InvalidOperationException(
                "Configuration key Jwt:PrivateKey does not contain a valid RSA private key.",
                ex);
        }

        if (_rsa.KeySize < 2048)
        {
            _rsa.Dispose();
            throw new InvalidOperationException(
                "The RSA private key must have a minimum size of 2048 bits.");
        }

        var keyId = CreateKeyId(_rsa);
        var signingKey = new RsaSecurityKey(_rsa) { KeyId = keyId };

        SigningCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.RsaSha256);
        ValidationKey = new RsaSecurityKey(_rsa.ExportParameters(false))
        {
            KeyId = keyId
        };
    }

    public RsaPublicJwk GetPublicJwk()
    {
        var parameters = _rsa.ExportParameters(false);

        return new RsaPublicJwk(
            Kty: "RSA",
            Use: "sig",
            Kid: SigningCredentials.Key.KeyId,
            Alg: SecurityAlgorithms.RsaSha256,
            N: Base64UrlEncoder.Encode(parameters.Modulus),
            E: Base64UrlEncoder.Encode(parameters.Exponent));
    }

    public void Dispose() => _rsa.Dispose();

    private static string NormalizePem(string privateKeyPem) =>
        privateKeyPem.Contains('\n')
            ? privateKeyPem
            : privateKeyPem.Replace("\\n", "\n", StringComparison.Ordinal);

    private static string CreateKeyId(RSA rsa)
    {
        var publicKey = rsa.ExportSubjectPublicKeyInfo();
        return Base64UrlEncoder.Encode(SHA256.HashData(publicKey));
    }
}
