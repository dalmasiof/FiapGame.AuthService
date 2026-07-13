using System.ComponentModel.DataAnnotations;

namespace DTOs
{
    public record CriarLoginDTOResponse(
        string Nome,
        string Email
        );
}
