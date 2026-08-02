using Microsoft.IdentityModel.Tokens;

namespace Interfaces;

public interface IRsaKeyProvider
{
    SigningCredentials SigningCredentials { get; }
    SecurityKey ValidationKey { get; }
    RsaPublicJwk GetPublicJwk();
}

public sealed record RsaPublicJwk(
    string Kty,
    string Use,
    string Kid,
    string Alg,
    string N,
    string E);
