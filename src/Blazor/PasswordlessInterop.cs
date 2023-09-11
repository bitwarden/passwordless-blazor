using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Passwordless.Blazor.Models;

namespace Passwordless.Blazor;

public class PasswordlessInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _passwordlessClientReferenceTask;
    private readonly ILogger<PasswordlessInterop> _logger;

    public PasswordlessInterop(IJSRuntime jsRuntime, IOptionsMonitor<PasswordlessClientOptions> optionsAccessor, ILogger<PasswordlessInterop> logger)
    {
        _logger = logger;
        var options = optionsAccessor.CurrentValue;
        _passwordlessClientReferenceTask = new Lazy<Task<IJSObjectReference>>(() => Initialize(jsRuntime, options.ApiKey, options.ApiUrl));

        // TODO: setup options callback
    }

    private static async Task<IJSObjectReference> Initialize(IJSRuntime jsRuntime, string apiKey, string apiUrl)
    {
        var module = await jsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/Passwordless.Blazor/Passwordless.Blazor.js");
        var passwordlessClient = await module.InvokeAsync<IJSObjectReference>("initializePasswordlessClient", apiKey, apiUrl);
        return passwordlessClient;
    }

    public async ValueTask<string> SigninWithIdAsync(string userId)
    {
        return await InvokeCoreAsync<TokenResponse, string>("signinWithId", r => r.Token!, userId);
    }

    public async ValueTask<string> SigninWithAliasAsync(string alias)
    {
        return await InvokeCoreAsync<TokenResponse, string>("signinWithAlias", r => r.Token!, alias);
    }

    public async ValueTask<string> SigninWithAutofillAsync()
    {
        return await InvokeCoreAsync<TokenResponse, string>("signinWithAutofill", r => r.Token!);
    }

    public async ValueTask<string> SigninWithDiscoverableAsync()
    {
        return await InvokeCoreAsync<TokenResponse, string>("signinWithDiscoverable", r => r.Token!);
    }

    public async ValueTask<string> RegisterAsync(string registerToken)
    {
        return await InvokeCoreAsync<TokenResponse, string>("register", r => r.Token!, registerToken);
    }

    private async ValueTask<TFinalReturn> InvokeCoreAsync<TReturn, TFinalReturn>(string identifier, Func<TReturn, TFinalReturn> returnCreator, params object?[]? args)
        where TReturn : IErrorContainer
    {
        var passwordlessClient = await _passwordlessClientReferenceTask.Value;
        try
        {
            var response = await passwordlessClient.InvokeAsync<TReturn>(identifier, args);
            if (response.Error is not null)
            {
                throw new PasswordlessClientException(response.Error);
            }

            return returnCreator(response);
        }
        catch (JSException ex)
        {
            _logger.LogError(ex, "An error occured during {Identifier}, please report this error so we can make a better product.", identifier);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_passwordlessClientReferenceTask.IsValueCreated)
        {
            var module = await _passwordlessClientReferenceTask.Value;
            await module.DisposeAsync();
        }
    }
}
