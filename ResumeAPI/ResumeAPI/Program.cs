using InfisicalConfiguration;
using ResumeAPI;
using ResumeAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Configuration.AddUserSecrets<Program>()
    .AddEnvironmentVariables();
Console.WriteLine("ClientId:" + builder.Configuration.GetSection("INFISICAL__CLIENTID").Value);
Console.WriteLine("ClientSecret:" + builder.Configuration.GetSection("INFISICAL__CLIENTSECRET").Value);
Console.WriteLine("ClientId:" + builder.Configuration.GetSection("ASPNETCORE_INFISICAL__CLIENTID").Value);
Console.WriteLine("ClientSecret:" + builder.Configuration.GetSection("ASPNETCORE_INFISICAL__CLIENTSECRET").Value);
var appSettings = builder.Configuration.Get<AppSettings>()!;
Console.WriteLine(appSettings.Infisical);
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddInfisical(new InfisicalConfigBuilder()
        .SetProjectId(appSettings.Infisical.ProjectId)
        .SetEnvironment(appSettings.Infisical.Environment)
        .SetInfisicalUrl(appSettings.Infisical.Url)
        .SetAuth(
            new InfisicalAuthBuilder().SetUniversalAuth(
                    appSettings.Infisical.ClientId,
                    appSettings.Infisical.ClientSecret)
                .Build())
        .Build()
    );
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, builder.Environment);