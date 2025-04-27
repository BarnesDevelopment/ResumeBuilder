using ResumeAPI;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
builder.Services.AddControllers();
builder.Configuration.AddUserSecrets<Program>();
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, builder.Environment);