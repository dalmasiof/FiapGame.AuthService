using Context;
using Entities;
using Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class LoginRepository(AuthContext _context) : ILoginRepository
    {
        public async Task AdicionarLogin(Login login)
        {
            await _context.Logins.AddAsync(login);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarLogin(Login login)
        {

            _context.Logins.Update(login);
            await _context.SaveChangesAsync();
        }

        public async Task<Login?> ObterLoginPorEmail(string email)
        {
            return await _context.Logins
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email);
        }

        public async Task<Login?> ObterLoginPorId(int id)
        {
            return await _context.Logins
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.IdLogin == id);
        }

        public async Task<IEnumerable<Login>> ObterLogins()
        {
            return await _context.Logins
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task TrocarSenha(Login login)
        {
            _context.Logins.Update(login);
            await _context.SaveChangesAsync();
        }
    }
}
