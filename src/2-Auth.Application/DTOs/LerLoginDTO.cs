using Entities;

namespace DTOs
{
    public record LerLoginDTO(
        int IdLogin,
        string Nome,
        string Email,
        string PasswordHash,
        string Ativo,
        TIPO_USUARIO TipoUsuario
    );
}
