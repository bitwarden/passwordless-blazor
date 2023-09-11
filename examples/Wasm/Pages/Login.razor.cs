using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

using Passwordless.AspNetCore.Client;
using Passwordless.Blazor;
using Passwordless.Examples.Wasm.Authentication;

namespace Passwordless.Examples.Wasm.Pages;

public partial class Login : ComponentBase
{
    public class LoginModel
    {
        public string Email { get; set; }
    }

    #nullable disable
    public LoginModel Model { get; set; }

    [Inject]
    public CustomAuthenticationStateProvider Auth { get; set; }
    #nullable enable

    public string? Error { get; set; }

    protected override void OnInitialized()
    {
        Model = new();
        base.OnInitialized();
    }

    public async Task LoginAsync(EditContext context)
    {
        await Auth.LoginAsync(Model.Email);
    }
}
