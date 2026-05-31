using DTOs;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(
        ITokenService tokenService,
        ILoginService loginService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(LogarLoginDTO logarLogin)
        {
            var usuario = await loginService.ValidarCredenciaisAsync(logarLogin);

            if (usuario == null)
            {
                return Unauthorized("Credenciais inválidas");
            }

            var token = tokenService.GerarToken(usuario);
            return Ok(new { Token = token });
        }

        [HttpPost("trocar-senha")]
        public async Task<IActionResult> TrocarSenha(TrocarSenhaDTO trocarSenha)
        {
            await loginService.TrocarSenhaAsync(trocarSenha);
            return Ok("Senha trocada com sucesso.");
        }
    }
}
