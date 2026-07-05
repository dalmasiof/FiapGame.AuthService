using DTOs;
using Entities;
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

            await _service.CriarLogin(dto);

            _repoMock.Verify(x => x.AdicionarLogin(It.IsAny<Login>()), Times.Once);
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