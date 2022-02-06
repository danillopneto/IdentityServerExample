using IdentityServerAuthentication;
using IdentityServerAuthentication.Configuration;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

try
{
    Log.Information("Apllication Starting up");
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.ConfigureIdentityServer(builder.Configuration);

    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    var identitySettings = builder.Configuration
                                  .GetSection(nameof(IdentityServerSettings))
                                  .Get<IdentityServerSettings>();
    var config = new Config(identitySettings);
    app.Services.InitializeDatabase(config);

    app.UseStaticFiles();
    app.UseRouting();
    app.UseIdentityServer();
    app.UseAuthorization();

    app.MapDefaultControllerRoute();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush();
}