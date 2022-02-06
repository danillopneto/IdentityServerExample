using IdentityServerAuthentication;
using IdentityServerAuthentication.Configuration;

var builder = WebApplication.CreateBuilder(args);

var identitySettings = builder.Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
var config = new Config(identitySettings);
builder.Services
       .AddIdentityServer()
       .AddInMemoryClients(config.GetClients())
       .AddInMemoryIdentityResources(config.IdentityResources)
       .AddInMemoryApiResources(config.ApiResources)
       .AddInMemoryApiScopes(config.ApiScopes)
       .AddTestUsers(config.Users)
       .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();