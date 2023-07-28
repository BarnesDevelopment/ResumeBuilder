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
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddTransient<IResumeOrchestrator, ResumeOrchestrator>();
        services.AddTransient<IResumeService, ResumeService>();
        
    #if DEBUG
        services.AddSassCompiler();
    #endif
    }
    public void Configure(WebApplication app, IWebHostEnvironment env) {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}