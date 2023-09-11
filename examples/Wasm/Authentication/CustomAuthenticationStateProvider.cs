using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Microsoft.Extensions.Http;

using Passwordless.AspNetCore.Client;
using Passwordless.Blazor;

namespace Passwordless.Examples.Wasm.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _client;
    private readonly PasswordlessInterop _passwordless;
    private readonly PasswordlessAspNetCoreClient _passwordlessClient;

    public CustomAuthenticationStateProvider(IHttpClientFactory clientFactory, PasswordlessInterop passwordless, PasswordlessAspNetCoreClient client)
    {
        _client = clientFactory.CreateClient(nameof(PasswordlessAspNetCoreClient));
        _passwordless = passwordless;
        _passwordlessClient = client;
    }
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "userinfo");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await _client.SendAsync(request);

            var rawClaims = await response.Content.ReadFromJsonAsync<List<KeyValuePair<string, string>>>();
            var claims = rawClaims!.Select(rc => new Claim(rc.Key, rc.Value));
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Api"));
            return new AuthenticationState(claimsPrincipal);
        }
        catch (HttpRequestException rEx) when (rEx.StatusCode == HttpStatusCode.Unauthorized)
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
        catch
        {
            // TODO: Log?
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }

    public async Task LoginAsync(string email)
    {
        var token = await _passwordless.SigninWithAliasAsync(email);

        await _passwordlessClient.LoginAsync(new PasswordlessLoginRequest(token));

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task LogoutAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "signout");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        var response = await _client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal())));
    }
}
