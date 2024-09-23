using System.IdentityModel.Tokens.Jwt;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace ResumeAPI;

public class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public void ConfigureServices(IServiceCollection services)
  {
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo
      {
        Title = "ResumeAPI",
        Version = "v1"
      });
      c.DocumentFilter<HealthChecksFilter>();

      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      c.IncludeXmlComments(xmlPath);
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
    services.AddTransient<IResumeBuilderService, ResumeBuilderBuilderService>();

    services.AddTransient<IUserOrchestrator, UserOrchestrator>();
    services.AddTransient<IUserService, UserService>();


    services.AddTransient<IUserData, UserData>();
    services.AddTransient<IResumeTree, ResumeTree>();

    services.AddTransient<IUserValidator, UserValidator>();

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidIssuer = appSettings.Jwt.Authority,
          ValidateAudience = false,
          ValidateIssuerSigningKey = false,
          ValidateLifetime = true,
          SignatureValidator = (token, parameters) => new JwtSecurityToken(token),
          RequireSignedTokens = false
        };

        options.Events = new JwtBearerEvents
        {
          OnAuthenticationFailed = context =>
          {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            Console.WriteLine("Authentication failed.");
            Console.WriteLine(context.Exception.Message);
            return Task.CompletedTask;
          },
          OnForbidden = context =>
          {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            Console.WriteLine("Forbidden. Role is likely incorrect.");
            return Task.CompletedTask;
          }
        };
      });

    services.AddAuthorization(options =>
    {
      options.AddPolicy("User", policy =>
        policy.RequireClaim("resume-id"));
    });

    services.AddHttpClient();

    var awsSecrets = _configuration.Get<AWSSecrets>();

    services.AddHealthChecks()
      .AddNpgSql(awsSecrets.ConnectionStrings_PostgreSql);

    #if DEBUG
    services.AddSassCompiler();
    #endif
  }

  public void Configure(WebApplication app, IWebHostEnvironment env)
  {
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseDeveloperExceptionPage();
      app.UseSwagger();
      app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ResumeAPI v1"));
    }


    app.UseStaticFiles();
    app.UseCors();

    app.UseHttpsRedirection();


    app.UseAuthentication();
    app.UseRouting();
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
