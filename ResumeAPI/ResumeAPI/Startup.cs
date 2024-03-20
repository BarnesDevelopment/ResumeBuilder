using System.Reflection;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ResumeAPI;

public class Startup
{
  private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration) {
        _configuration = configuration;
    }
    public void ConfigureServices(IServiceCollection services) {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ResumeAPI", 
                Version = "v1",
            });
            c.DocumentFilter<HealthChecksFilter>();
            
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            
            c.AddSecurityDefinition("User cookie", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Description = "User cookie",
                Name = "Authorization",
                In = ParameterLocation.Header,
            });
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                policy =>
                {
                    policy.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost").AllowAnyHeader().AllowAnyMethod();
                });
        });
        
        services.Configure<AWSSecrets>(_configuration);
        var appSettings = _configuration.Get<AppSettings>();

        services.AddTransient<IResumeOrchestrator, ResumeOrchestrator>();
        services.AddTransient<IResumeService, ResumeService>();
        
        services.AddTransient<IUserOrchestrator, UserOrchestrator>();
        services.AddTransient<IUserService, UserService>();
        
        services.AddTransient<IUserData, UserData>();
        services.AddTransient<IResumeTree, ResumeTree>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<ICookieValidator, CookieValidator>();
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
            options.Authority = appSettings.Jwt.Authority;
          });

        services.AddHttpClient();
        
        var awsSecrets = _configuration.Get<AWSSecrets>();

        services.AddHealthChecks()
            .AddNpgSql(awsSecrets.ConnectionStrings_PostgreSql);
        
    #if DEBUG
        services.AddSassCompiler();
    #endif
    }
    public void Configure(WebApplication app, IWebHostEnvironment env) {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ResumeAPI v1"));
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthcheck", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });

        app.Run();
    }
}
