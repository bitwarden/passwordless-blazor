using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Passwordless.AspNetCore.Client;

public record PasswordlessAddCredentialRequest(string? DisplayName);
public record PasswordlessLoginRequest(string Token);
public class PasswordlessRegisterRequest
{
    public string Username { get; }
    public string? DisplayName { get; }
    public string? Email { get; set; }
    public HashSet<string>? Aliases { get; }

    public PasswordlessRegisterRequest(string username, string? displayName, HashSet<string>? aliases)
    {
        Username = username;
        DisplayName = displayName;
        Aliases = aliases;
    }
}

public record TokenResponse(string Token);

public class PasswordlessAspNetCoreClient : PasswordlessAspNetCoreClient<PasswordlessRegisterRequest>
{
    public PasswordlessAspNetCoreClient(HttpClient client) : base(client)
    {
    }
}

public class PasswordlessAspNetCoreClient<TRegisterBody>
{
    private readonly HttpClient _client;

    public PasswordlessAspNetCoreClient(HttpClient client)
    {
        _client = client;
    }

    public async Task LoginAsync(PasswordlessLoginRequest loginRequest, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "passwordless-login");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        request.Content = JsonContent.Create(loginRequest);
        var response = await _client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<TokenResponse> RegisterAsync(TRegisterBody registerRequest, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("passwordless-register", registerRequest, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken))!;
    }

    public async Task<TokenResponse> AddCredentialAsync(PasswordlessAddCredentialRequest addCredentialRequest, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("passwordless-add-credential", addCredentialRequest, cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken))!;
    }
}
