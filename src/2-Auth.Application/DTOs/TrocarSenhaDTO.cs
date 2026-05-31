using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public record TrocarSenhaDTO
    (
        [Required(ErrorMessage = "Credenciais inválidas")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "E-mail inválido")]
        string Email,

        [Required(ErrorMessage = "Credenciais inválidas")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
            ErrorMessage = "Não atende aos critérios de segurança")]
        string NovaSenha
    );
}
