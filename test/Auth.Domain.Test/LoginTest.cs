using Bogus;
using Entities;

namespace Auth.Domain.Test
{
    public class LoginTests
    {
        private readonly Faker _faker = new Faker("pt_BR");

        [Fact]
        public void Deve_Criar_Login_Valido()
        {
            var login = CriarLogin();

            Assert.NotNull(login);
            Assert.True(login.Ativo);
            Assert.True(login.DataCriacao <= DateTime.UtcNow);
        }

        [Fact]
        public void Deve_Atualizar_Dados_Do_Login()
        {
            var login = CriarLogin();

            var novoNome = _faker.Name.FullName();
            var novoEmail = _faker.Internet.Email();
            var novaSenha = _faker.Internet.Password();

            login.AtualizarLogin(novoNome, novoEmail, novaSenha);

            Assert.Equal(novoNome, login.Nome);
            Assert.Equal(novoEmail, login.Email);
            Assert.Equal(novaSenha, login.PasswordHash);
        }

        [Fact]
        public void Deve_Desativar_Login()
        {
            var login = CriarLogin();

            login.DesativarLogin();

            Assert.False(login.Ativo);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Nome_Vazio()
        {
            var email = _faker.Internet.Email();
            var senha = _faker.Internet.Password();

            var ex = Assert.Throws<ArgumentException>(() =>
                new Login("", email, senha, 1));

            Assert.Equal("O nome é obrigatório.", ex.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Nome_Nulo()
        {
            var email = _faker.Internet.Email();
            var senha = _faker.Internet.Password();

            var ex = Assert.Throws<ArgumentException>(() =>
                new Login(null, email, senha, 1));

            Assert.Equal("O nome é obrigatório.", ex.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Email_Invalido_Sem_Arroba()
        {
            var nome = _faker.Name.FullName();
            var senha = _faker.Internet.Password();

            var ex = Assert.Throws<ArgumentException>(() =>
                new Login(nome, "email-invalido", senha, 1));

            Assert.Equal("E-mail com formato inválido.", ex.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Email_Invalido_Incompleto()
        {
            var nome = _faker.Name.FullName();
            var senha = _faker.Internet.Password();

            var ex = Assert.Throws<ArgumentException>(() =>
                new Login(nome, "teste@", senha, 1));

            Assert.Equal("E-mail com formato inválido.", ex.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Email_Vazio()
        {
            var nome = _faker.Name.FullName();
            var senha = _faker.Internet.Password();

            var ex = Assert.Throws<ArgumentException>(() =>
                new Login(nome, "", senha, 1));

            Assert.Equal("O email é obrigatório.", ex.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Email_Nulo()
        {
            var nome = _faker.Name.FullName();
            var senha = _faker.Internet.Password();

            var ex = Assert.Throws<ArgumentException>(() =>
                new Login(nome, null, senha, 1));

            Assert.Equal("O email é obrigatório.", ex.Message);
        }

        [Fact]
        public void Deve_Lancar_Excecao_Quando_Senha_Vazia()
        {
            var nome = _faker.Name.FullName();
            var email = _faker.Internet.Email();

            Assert.Throws<ArgumentException>(() =>
                new Login(nome, email, "", 1));
        }

        private Login CriarLogin()
        {
            return new Login(
                _faker.Name.FullName(),
                _faker.Internet.Email(),
                _faker.Internet.Password(),
                1
            );
        }
    }
}