using Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Context
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AuthContext context, IConfiguration configuration)
        {
            if (await context.Logins.AnyAsync())
                return;

            var nome = configuration["BootstrapAdmin:Name"];
            var email = configuration["BootstrapAdmin:Email"];
            var password = configuration["BootstrapAdmin:Password"];

            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException(
                    "O banco de autenticação está vazio. Configure BootstrapAdmin:Name, BootstrapAdmin:Email e BootstrapAdmin:Password em um provedor seguro de segredos para criar o administrador inicial.");
            }

            var admin = new Login(nome, email, BCrypt.Net.BCrypt.HashPassword(password), (int)TIPO_USUARIO.Admin);
            context.Logins.Add(admin);
            await context.SaveChangesAsync();
        }
    }
}
