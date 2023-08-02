using ResumeAPI;
using ResumeAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
builder.Services.AddControllers();
startup.ConfigureServices(builder.Services);
builder.Host.ConfigureAppConfiguration(((_, configurationBuilder) =>
{
    var secretName = _.Configuration.GetSection("AzureSecret").Value;
    var region = _.Configuration.GetSection("AzureRegion").Value;
    configurationBuilder.AddAmazonSecretsManager(region, secretName);
}));
var app = builder.Build();
startup.Configure(app, builder.Environment);



