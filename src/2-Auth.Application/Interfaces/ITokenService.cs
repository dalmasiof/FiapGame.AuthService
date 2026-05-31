using DTOs;

namespace Interfaces
{
    public interface ITokenService
    {
        string GerarToken(LerLoginDTO login);
    }
}