using ResumeAPI;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
builder.Services.AddControllers();
startup.ConfigureServices(builder.Services);
var app = builder.Build();
startup.Configure(app, builder.Environment);

