using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
[AllowAnonymous]
[Route(".well-known/openid-configuration")]
public sealed class OpenIdConfigurationController(IConfiguration configuration) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var forwardedScheme = Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
        var scheme = string.IsNullOrWhiteSpace(forwardedScheme)
            ? Request.Scheme
            : forwardedScheme.Split(',', StringSplitOptions.TrimEntries)[0];
        var baseUrl = $"{scheme}://{Request.Host}{Request.PathBase}";

        return Ok(new
        {
            issuer = configuration["Jwt:Issuer"],
            jwks_uri = $"{baseUrl}/.well-known/jwks",
            id_token_signing_alg_values_supported = new[] { "RS256" }
        });
    }
}
