using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Passwordless.Examples.Wasm;
using Passwordless.Examples.Wasm.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddPasswordlessAspNetCoreClient(options =>
{
    // Default url for Passwordless.AspNetCore Example, to be committed
    options.BackendUrl = "http://localhost:5260";
});

builder.Services.AddPasswordlessBlazor(options =>
{
    options.ApiKey = "mydemo:public:8f48704d2f23461797f3196f627aa6f6";
});

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

await builder.Build().RunAsync();
