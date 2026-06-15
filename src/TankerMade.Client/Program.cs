using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using MudBlazor.Services;
using TankerMade.Client;
using TankerMade.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<AuthSession>();
builder.Services.AddScoped<TankerMadeApiClient>();
builder.Services.AddScoped<ClientModuleState>();
builder.Services.AddScoped<CoreRecentModule>();
builder.Services.AddScoped<KnittingRecentActivity>();
builder.Services.AddScoped<KnittingRowProgress>();
builder.Services.AddScoped<KnittingCardAssetCache>();
builder.Services.AddScoped<ThemeService>();

await builder.Build().RunAsync();
