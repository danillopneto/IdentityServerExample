using IdentityServerAuthentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services
       .AddIdentityServer()
       .AddInMemoryClients(Config.Clients)
       .AddInMemoryIdentityResources(Config.IdentityResources)
       .AddInMemoryApiResources(Config.ApiResources)
       .AddInMemoryApiScopes(Config.ApiScopes)
       .AddTestUsers(Config.Users)
       .AddDeveloperSigningCredential();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();