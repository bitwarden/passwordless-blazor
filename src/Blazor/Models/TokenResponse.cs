namespace Passwordless.Blazor.Models;

internal class TokenResponse : IErrorContainer
{
    public string? Token { get; set; }
    public PasswordlessProblemDetails? Error { get; set; }
}
