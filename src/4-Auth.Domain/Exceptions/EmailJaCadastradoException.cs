namespace Exceptions;

public sealed class EmailJaCadastradoException()
    : Exception("Este e-mail já possui um cadastro. Use outro e-mail ou faça login.")
{
}
