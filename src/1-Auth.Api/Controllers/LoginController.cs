using DTOs;
using Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(ILoginService loginService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CriarLogin([FromBody] CriarLoginDTO loginDTO)
        {
            CriarLoginDTO novoLogin = await loginService.CriarLogin(loginDTO);
            return Ok(novoLogin);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await loginService.ObterLoginPorId(id);
            return Ok(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("email/{email}")]
        public async Task<IActionResult> ObterPorEmail(string email)
        {
            var usuario = await loginService.ObterLoginPorEmail(email);
            return Ok(usuario);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ObterTodosLogins()
        {
            var usuarios = await loginService.ObterLogins();
            return Ok(usuarios);
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> AtualizarLogin([FromBody] AtualizarLoginDTO loginDTO)
        {
            await loginService.AtualizarLogin(loginDTO);
            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletarLogin(int id)
        {
            await loginService.DeletarLogin(id);
            return Ok();
        }
    }
}