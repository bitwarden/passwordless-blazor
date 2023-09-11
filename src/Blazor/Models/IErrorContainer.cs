namespace Passwordless.Blazor.Models;

internal interface IErrorContainer
{
    PasswordlessProblemDetails? Error { get; }
}
