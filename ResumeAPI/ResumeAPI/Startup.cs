using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using ResumeAPI.Database;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace ResumeAPI;

public class Startup
{
    public IConfiguration configRoot {
        get;
    }
    public Startup(IConfiguration configuration) {
        configRoot = configuration;
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
        });

        services.Configure<AWSSecrets>(configRoot);

        services.AddTransient<IResumeOrchestrator, ResumeOrchestrator>();
        services.AddTransient<IResumeService, ResumeService>();
        
        services.AddTransient<IUserOrchestrator, UserOrchestrator>();
        services.AddTransient<IUserService, UserService>();
        
        services.AddTransient<IMySqlContext, MySqlContext>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        
        services.AddHttpClient();

        var mysql = configRoot.GetConnectionString("mysql");
        services.AddHealthChecks()
            .AddMySql(configRoot.GetConnectionString("mysql")!);
        
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

        app.UseHttpsRedirection();

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