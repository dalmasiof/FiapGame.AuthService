using Entities;

namespace Interfaces
{
    public interface ILoginRepository
    {
        Task<Login?> ObterLoginPorId(int id);
        Task<Login?> ObterLoginPorEmail(string email);
        Task<IEnumerable<Login>> ObterLogins();
        Task AdicionarLogin(Login login);
        Task AtualizarLogin(Login login);
        Task TrocarSenha(Login login);
    }
}
