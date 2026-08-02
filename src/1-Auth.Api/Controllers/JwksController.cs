using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
[AllowAnonymous]
[Route(".well-known/jwks")]
public sealed class JwksController(IRsaKeyProvider rsaKeyProvider) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() =>
        Ok(new { keys = new[] { rsaKeyProvider.GetPublicJwk() } });
}
