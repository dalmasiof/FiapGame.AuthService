using Entities;

namespace Context
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AuthContext context)
        {
            try
            {
                //se já tem dados, não faz nada
                if (context.Logins.Any())
                    return;

                var admin = new Login("admin", "admin@admin.com", BCrypt.Net.BCrypt.HashPassword("Admin@123"), 0);
                
                var usuario = new Login("user", "user@user.com", BCrypt.Net.BCrypt.HashPassword("User@123"), 1);

                context.Logins.AddRange(admin, usuario);
                await context.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
            
        }
    }
}
