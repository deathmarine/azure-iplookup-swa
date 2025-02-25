using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BlazorApp.Client;
using MudBlazor.Services;
using BlazorApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddSingleton(sp => new HttpClient { 
    BaseAddress = new Uri(builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) 
});
if (builder.HostEnvironment.Environment == "Development") {
    //Named client for local development and connects the Frontend SWA to the Backend FA in Visual Studio.
    builder.Services.AddHttpClient("dev_api", c => {
        c.BaseAddress = new Uri("http://localhost:7071/");
    });
    builder.Services.AddSingleton<IAPIService, APITestService>(); //Local Development
} else {
    builder.Services.AddHttpClient("api", c => {
        c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    });
    builder.Services.AddSingleton<IAPIService, APIService>(); //Production
}

await builder.Build().RunAsync();
