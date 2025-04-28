using InfisicalConfiguration;
using ResumeAPI;
using ResumeAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Configuration.AddUserSecrets<Program>()
    .AddEnvironmentVariables();
var appSettings = builder.Configuration.Get<AppSettings>()!;
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