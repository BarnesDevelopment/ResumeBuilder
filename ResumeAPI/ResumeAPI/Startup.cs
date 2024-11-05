using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using ResumeAPI.Database;
using ResumeAPI.DemoCookieAuth;
using ResumeAPI.Helpers;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

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
        var dev = _configuration.GetSection("Environment").Value == "Development";

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "ResumeAPI", Version = "v1" });
            c.DocumentFilter<HealthChecksFilter>();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        services.Configure<AWSSecrets>(_configuration);
        var appSettings = _configuration.Get<AppSettings>();
        var awsSecrets = _configuration.Get<AWSSecrets>();

        if (dev)
        {
            Console.WriteLine("Development mode enabled.");
            Console.WriteLine("PostgreSql connection string: {0}", awsSecrets.ConnectionStrings_PostgreSql);
        }

        #region Dependency Injection

        services.AddTransient<IDemoOrchestrator, DemoOrchestrator>();

        services.AddTransient<IResumeOrchestrator, ResumeOrchestrator>();
        services.AddTransient<IResumeBuilderService, ResumeBuilderBuilderService>();
        services.AddTransient<IResumeService, ResumeService>();

        services.AddTransient<IUserOrchestrator, UserOrchestrator>();
        services.AddTransient<IUserService, UserService>();

        services.AddTransient<IUserData, UserData>();
        services.AddTransient<IResumeTree, ResumeTree>();

        services.AddTransient<IUserValidator, UserValidator>();
        services.AddTransient<IAnonymousUserValidator, AnonymousUserValidator>();

        #endregion

        #region Auth

        #region Authentication

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

        services.AddAuthentication(Constants.DemoCookieAuth).UseDemoCookieAuthentication();

        #endregion

        #region Authorization

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = "MultiAuth";
                options.DefaultChallengeScheme = "MultiAuth";
            })
            .AddPolicyScheme("MultiAuth",
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        var authorization = context.Request.Headers[HeaderNames.Authorization].ToString();
                        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer"))
                            return JwtBearerDefaults.AuthenticationScheme;
                        return Constants.DemoCookieAuth;
                    };
                });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("User",
                policy =>
                    policy.RequireClaim("resume-id"));
        });

        #endregion

        #region Cors

        services.AddCors(options =>
        {
            options.AddPolicy(Constants.Cors.Development,
                policy =>
                {
                    policy.SetIsOriginAllowed(x => new Uri(x).Host == "localhost")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });

            options.AddPolicy(Constants.Cors.Production,
                policy =>
                {
                    policy.WithOrigins("https://resume-builder.barnes-development.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        #endregion

        #endregion

        services.AddHttpClient();

        services.AddHealthChecks()
            .AddNpgSql(awsSecrets.ConnectionStrings_PostgreSql);

        #if DEBUG
        services.AddSassCompiler();
        #endif
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ResumeAPI v1"));
            app.UseCors(Constants.Cors.Development);
        }
        else
            app.UseCors(Constants.Cors.Production);

        app.UseStaticFiles();
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthcheck",
                new HealthCheckOptions
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