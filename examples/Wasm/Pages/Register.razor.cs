

using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Components;

using Passwordless.AspNetCore.Client;
using Passwordless.Blazor;

namespace Passwordless.Examples.Wasm.Pages;

public partial class Register : ComponentBase
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Nickname { get; set; }
    }

    #nullable disable
    [Inject]
    public PasswordlessAspNetCoreClient Client { get; set; }

    [Inject]
    public PasswordlessInterop Passwordless { get; set; }
    public RegisterModel Model { get; set; }
    #nullable enable

    public string? Error { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        Model = new();
    }

    public async Task RegisterAsync()
    {
        try
        {
            var response = await Client.RegisterAsync(
                new PasswordlessRegisterRequest(Model.Email, Model.Nickname, new HashSet<string> { Model.Email }));

            var token = await Passwordless.RegisterAsync(response.Token);
        }
        catch (PasswordlessClientException ex)
        {
            Error = ex.Message;
        }
    }
}
