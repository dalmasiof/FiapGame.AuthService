using DTOs;
using Entities;
using IntegrationEvents;
using Interfaces;
using Moq;
using Services;

namespace Auth.Application.Test
{
    public class LoginServicoTests
    {
        private readonly Mock<ILoginRepository> _repoMock;
        private readonly Mock<IMessagePublisher> _publisherMock;
        private readonly LoginService _service;

        public LoginServicoTests()
        {
            _repoMock = new Mock<ILoginRepository>();
            _publisherMock = new Mock<IMessagePublisher>();
            _service = new LoginService(_repoMock.Object, _publisherMock.Object);
        }

        [Fact]
        public async Task Deve_Criar_Login()
        {
            var dto = new CriarLoginDTO("Nome", "email@test.com", "123", 1);
            _repoMock.Setup(x => x.AdicionarLogin(It.IsAny<Login>()))
                .Callback<Login>(login => login.IdLogin = 42)
                .Returns(Task.CompletedTask);

            await _service.CriarLogin(dto);

            _repoMock.Verify(x => x.AdicionarLogin(It.IsAny<Login>()), Times.Once);
            _publisherMock.Verify(
                x => x.PublishAsync(
                    It.IsAny<NotificacaoIntegrationEvent>(),
                    "autenticacao.notificacao",
                    It.IsAny<string>()),
                Times.Once);
            _publisherMock.Verify(
                x => x.PublishAsync(
                    It.Is<UsuarioRegistradoIntegrationEvent>(evento =>
                        evento.IdLogin == 42 &&
                        evento.Nome == dto.Nome &&
                        evento.Email == dto.Email &&
                        evento.TipoUsuario == dto.TipoUsuario),
                    "autenticacao.usuario.registrado",
                    It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Deve_Lancar_Excecao_Quando_Login_Nao_Encontrado()
        {
            _repoMock.Setup(x => x.ObterLoginPorId(It.IsAny<int>()))
                     .ReturnsAsync((Login)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.ObterLoginPorId(1));
        }
    }
}