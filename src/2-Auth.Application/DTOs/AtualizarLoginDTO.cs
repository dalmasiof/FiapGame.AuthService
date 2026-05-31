using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public record AtualizarLoginDTO(
        [Required(ErrorMessage = "O ID do login é obrigatório.")]
        int IdLogin,

        [Required(ErrorMessage = "O nome é obrigatório.")]
        string Nome,

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "E-mail inválido.")]
        string Email,

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
            ErrorMessage = "A senha deve ter no mínimo 8 caracteres, contendo letras maiúsculas, minúsculas, números e pelo menos um caractere especial.")]
        string PasswordHash
    );
}
