using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using FluentValidation;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResumeAPI.Database;
using ResumeAPI.Models;
using ResumeAPI.Orchestrator;
using ResumeAPI.Services;

namespace ResumeAPI;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        var dev = new[] { "Development", "Docker" }.Any(x => x == configuration.GetSection("Environment").Value);

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

        var appSettings = configuration.Get<AppSettings>();
        if (appSettings == null) throw new InvalidOperationException("AppSettings cannot be null.");

        if (dev)
        {
            Console.WriteLine("Development mode enabled.");
            Console.WriteLine("PostgreSql connection string: {0}", appSettings.ConnectionStrings.Postgres);
        }

        #region Dependency Injection

        services.AddSingleton(appSettings);
        services.AddTransient<IDemoOrchestrator, DemoOrchestrator>();

        services.AddTransient<IResumeOrchestrator, ResumeOrchestrator>();
        services.AddTransient<IResumeBuilderService, ResumeBuilderBuilderService>();
        services.AddTransient<IResumeService, ResumeService>();

        services.AddTransient<IUserService, UserService>();

        services.AddTransient<IResumeTree, ResumeTree>();

        services.AddScoped<IAuthClient, AuthClient>();

        services.AddHttpClient<IAuthClient, AuthClient>(options =>
        {
            options.BaseAddress = new Uri("https://" + appSettings.Jwt.Authority);
        });

        services.AddValidatorsFromAssembly(Assembly.Load("ResumeAPI"), ServiceLifetime.Transient);

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
                    OnTokenValidated = async ctx =>
                    {
                        var authClient = ctx.HttpContext.RequestServices.GetRequiredService<IAuthClient>();
                        var jwt = ctx.SecurityToken as JwtSecurityToken;
                        if (jwt == null)
                        {
                            ctx.Fail("Invalid security token.");
                            return;
                        }

                        var remoteResult = await authClient.AuthenticateJwt(jwt.RawData);
                        if (!remoteResult) ctx.Fail("Remote token validation failed.");
                    }
                };
            });

        #endregion

        #region Authorization

        services.AddAuthorization(options =>
        {
            options.AddPolicy("User",
                policy =>
                    policy.RequireClaim("userId"));
        });

        #endregion

        #region Cors

        services.AddCors(options =>
        {
            options.AddPolicy(Constants.Cors.Development,
                policy =>
                {
                    policy.SetIsOriginAllowed(x => new Uri(x).Host == "localhost")
                        .AllowCredentials()
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
            .AddNpgSql(appSettings.ConnectionStrings.Postgres);

        #if DEBUG
        services.AddSassCompiler();
        #endif
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsEnvironment("Development") || env.IsEnvironment("Docker"))
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