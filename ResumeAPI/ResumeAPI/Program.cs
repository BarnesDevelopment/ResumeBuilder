using ResumeAPI;
using ResumeAPI.Configuration;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
builder.Services.AddControllers();
builder.Host.ConfigureAppConfiguration(((_, configurationBuilder) =>
{
    var secretName = _.Configuration.GetSection("AzureSecret").Value;
    var region = _.Configuration.GetSection("AzureRegion").Value;
    configurationBuilder.AddAmazonSecretsManager(region, secretName);
}));
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, builder.Environment);



