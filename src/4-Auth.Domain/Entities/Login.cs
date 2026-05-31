using System.Text.RegularExpressions;

namespace Entities
{
    public class Login
    {
        public int IdLogin { get; set; }
        public int IdConta { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Ativo { get; set; }
        public TIPO_USUARIO TipoUsuario { get; set; }
        protected Login() { }
        public Login(string nome, string email, string passwordHash, int tipoUsuario)
        {
            if (string.IsNullOrEmpty(nome))
                throw new ArgumentException("O nome é obrigatório.");

            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("O email é obrigatório.");

            if (string.IsNullOrEmpty(passwordHash))
                throw new ArgumentException("A senha é obrigatória.");

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                throw new ArgumentException("E-mail com formato inválido.");

            Nome = nome;
            Email = email;
            PasswordHash = passwordHash;
            DataCriacao = DateTime.UtcNow;
            Ativo = true;
            TipoUsuario = (TIPO_USUARIO)tipoUsuario;

        }

        public void AtualizarLogin(string nome, string email, string passwordHash)
        {
            Nome = nome;
            Email = email; 
            PasswordHash = passwordHash;
        }

        public void DesativarLogin()
        {
            Ativo = false;
        }

        public void TrocarSenha(string novaSenhaHash)
        {
            PasswordHash = novaSenhaHash;
        }
    }
}