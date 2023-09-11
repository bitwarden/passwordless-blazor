namespace Passwordless.Blazor;

public class PasswordlessClientOptions
{

    public string ApiKey { get; set; } = null!;
    public string ApiUrl { get; set; } = "https://v4.passwordless.dev";

    // TODO: origin & rpid?
}
