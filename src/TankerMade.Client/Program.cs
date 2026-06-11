using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using TankerMade.Client;
using TankerMade.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? builder.HostEnvironment.BaseAddress;
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<AuthSession>();
builder.Services.AddScoped<TankerMadeApiClient>();
builder.Services.AddScoped<ClientModuleState>();
builder.Services.AddScoped<KnittingRecentActivity>();
builder.Services.AddScoped<KnittingCardAssetCache>();

await builder.Build().RunAsync();
