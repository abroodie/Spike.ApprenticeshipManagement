using ApprenticeManagement.POC.Common;
using ApprenticeManagement.POC.Web;
using ApprenticeManagement.POC.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<DeviceManagementServiceClient>(svc => new DeviceManagementServiceClient(svc.GetService<IConfiguration>()["apprenticeManagementServiceBaseUri"]));
builder.Services.AddSingleton<ApprenticeManagementServiceClient>(svc => new ApprenticeManagementServiceClient(svc.GetService<IConfiguration>()["apprenticeManagementServiceBaseUri"]));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
