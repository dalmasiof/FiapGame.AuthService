using DTOs;
using Entities;
using IntegrationEvents;
using Interfaces;

namespace Services
{
    public class LoginService(ILoginRepository loginRepository, IMessagePublisher _publisher) : ILoginService
    {
        private const string NotificacaoRoutingKey = "autenticacao.notificacao";
        private const string UsuarioRegistradoRoutingKey = "autenticacao.usuario.registrado";

        public async Task<CriarLoginDTOResponse> CriarLogin(CriarLoginDTO loginDTO)
        {
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(loginDTO.PasswordHash);

            var novoLogin = new Login(loginDTO.Nome, loginDTO.Email, senhaHash, (int)loginDTO.TipoUsuario);

            await loginRepository.AdicionarLogin(novoLogin);

            var eventoNotificacao = new NotificacaoIntegrationEvent(
                CorrelacaoId: Guid.NewGuid(),
                Destinatario: novoLogin.Email,
                Assunto: "Bem-vindo ao FiapGames!",
                CorpoMensagem: $"Olá {novoLogin.Nome}, sua conta foi criada com sucesso.",
                DominioOrigem: "Autenticacao"
            );

            var eventoUsuarioRegistrado = new UsuarioRegistradoIntegrationEvent(
                IdLogin: novoLogin.IdLogin,
                Nome: novoLogin.Nome,
                Email: novoLogin.Email,
                TipoUsuario: (int)novoLogin.TipoUsuario);

            await _publisher.PublishAsync(eventoNotificacao, NotificacaoRoutingKey, eventoNotificacao.RastreioId);
            await _publisher.PublishAsync(eventoUsuarioRegistrado, UsuarioRegistradoRoutingKey, Guid.NewGuid().ToString());

            return new CriarLoginDTOResponse
            (
                novoLogin.Nome,
                novoLogin.Email
            );
        }

        public async Task<LerLoginDTO?> ObterLoginPorId(int id)
        {
            var login = await loginRepository.ObterLoginPorId(id);
            if (login == null) throw new ArgumentException("Login não encontrado", nameof(id));

            return new LerLoginDTO
            (
                login.IdLogin,
                login.Nome,
                login.Email,
                login.PasswordHash = "",
                login.Ativo ? "Sim" : "Não",
                login.TipoUsuario
            );
        }

        public async Task<LerLoginDTO> ObterLoginPorEmail(string email)
        {
            var login = await loginRepository.ObterLoginPorEmail(email);
            if (login == null) throw new ArgumentException("Login não encontrado");

            return new LerLoginDTO
            (
                login.IdLogin,
                login.Nome,
                login.Email,
                login.PasswordHash = "",
                login.Ativo ? "Sim" : "Não",
                login.TipoUsuario
            );
        }

        public async Task<IEnumerable<LerLoginDTO>> ObterLogins()
        {
            List<LerLoginDTO> logins = new List<LerLoginDTO>();
            foreach (var login in await loginRepository.ObterLogins())
            {
                logins.Add(new LerLoginDTO
                (
                    login.IdLogin,
                    login.Nome,
                    login.Email,
                    login.PasswordHash = "",
                    login.Ativo ? "Sim" : "Não",
                    login.TipoUsuario
                ));
            }
            return logins;
        }

        public async Task AtualizarLogin(AtualizarLoginDTO loginDTO)
        {
            var login = await loginRepository.ObterLoginPorId(loginDTO.IdLogin);
            string senhaHash = BCrypt.Net.BCrypt.HashPassword(loginDTO.PasswordHash);

            if (login == null) throw new ArgumentException("Login não encontrado");

            login.AtualizarLogin(loginDTO.Nome, loginDTO.Email, senhaHash);
            await loginRepository.AtualizarLogin(login);
        }

        public async Task DeletarLogin(int id)
        {
            var login = await loginRepository.ObterLoginPorId(id);

            if (login == null) throw new ArgumentException("Login não encontrado");

            login.DesativarLogin();

            await loginRepository.AtualizarLogin(login);
        }

        public async Task<LerLoginDTO> ValidarCredenciaisAsync(LogarLoginDTO logarLogin)
        {
            var login = await loginRepository.ObterLoginPorEmail(logarLogin.Email);
            if (login == null)
            {
                throw new ArgumentException("Credenciais inválidas");
            }
            bool senhaValida = BCrypt.Net.BCrypt.Verify(logarLogin.PasswordHash, login.PasswordHash);
            if (!senhaValida)
            {
                throw new ArgumentException("Credenciais inválidas");
            }

            return new LerLoginDTO
            (
                login.IdLogin,
                login.Nome,
                login.Email,
                login.PasswordHash = "",
                login.Ativo ? "Sim" : "Não",
                login.TipoUsuario
            );
        }

        public async Task TrocarSenhaAsync(TrocarSenhaDTO trocarSenha)
        {
            var login = await loginRepository.ObterLoginPorEmail(trocarSenha.Email);
            if (login == null)
            {
                throw new ArgumentException("E-mail não encontrado");
            }

            string novaSenhaHash = BCrypt.Net.BCrypt.HashPassword(trocarSenha.NovaSenha);
            login.TrocarSenha(novaSenhaHash);
            await loginRepository.TrocarSenha(login);
        }
    }
}
