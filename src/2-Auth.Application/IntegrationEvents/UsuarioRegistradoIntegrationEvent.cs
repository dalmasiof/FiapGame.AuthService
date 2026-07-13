namespace IntegrationEvents;

public record UsuarioRegistradoIntegrationEvent(
    int IdLogin,
    string Nome,
    string Email,
    int TipoUsuario // 0 Admin, 1 Usuario (conforme seu modelo)
);