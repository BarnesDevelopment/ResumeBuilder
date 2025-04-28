using InfisicalConfiguration;
using ResumeAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Configuration.AddUserSecrets<Program>()
    .AddEnvironmentVariables()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddInfisical(new InfisicalConfigBuilder()
        .SetProjectId("39828bb7-07bb-4f99-bc16-706caf452bde")
        .SetEnvironment("dev")
        .SetInfisicalUrl("https://secrets.barnes7619.com")
        .SetAuth(
            new InfisicalAuthBuilder().SetUniversalAuth(
                    builder.Configuration.GetRequiredSection("InfisicalClientId").Value!,
                    builder.Configuration.GetRequiredSection("InfisicalClientSecret").Value!)
                .Build())
        .Build()
    );
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, builder.Environment);