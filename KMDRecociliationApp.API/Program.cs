using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using KMDRecociliationApp.Data;
using KMDRecociliationApp.Data.Repositories;
using Serilog;
using KMDRecociliationApp.API.Common;
using KMDRecociliationApp.Domain.ConfigurationModels;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using KMDRecociliationApp.Domain.DTO;
using KMDRecociliationApp.Domain.PaymentDTO;
using KMDRecociliationApp.API.Services;
using UserActivityMonitoringService.Middleware;
using UserActivityMonitoringService.Services;
using SixLabors.ImageSharp;
using UserActivityMonitoringService.Data;
using KMDRecociliationApp.Domain.WhatsappDTO;
using KMDRecociliationApp.Services;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using KMDRecociliationApp.Data.Repositories.DataCollectionrepo;
using KMDRecociliationApp.Data.Repositories.Interfaces.KMD.EnrolmentPortal.Repositories.Interfaces;
using KMDRecociliationApp.Data.Repositories.Interfaces;
using KMDRecociliationApp.Data.Services.Interfaces;
using KMDRecociliationApp.Data.Services;
using Microsoft.AspNetCore.Hosting;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();
try
{


    var builder = WebApplication.CreateBuilder(args);
    var connstr = builder.Configuration.GetConnectionString("constr");
    var userActivityConnectionstr = builder.Configuration.GetConnectionString("UserActivityConnection");
    Log.Information("Starting up");
    builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 4;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2;
    }));

    //Add support to logging with SERILOG
    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));
    var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

    builder.Services.AddCors(c =>
     c.AddPolicy("AllowOrigin", options =>
     {
         options.AllowAnyOrigin();
         options.AllowAnyMethod();
         options.AllowAnyHeader();

         options.WithOrigins(allowedOrigins);
        
         options.WithExposedHeaders("Content-Disposition");
     }));

    var key = Encoding.ASCII.GetBytes("1bb6e0ae1df38486322853ee0d8278bc88b6a57ea49f62d6a1d0d7f517f69207");

    // Register the HttpClient service
    builder.Services.AddHttpClient();
   
    builder.Services.AddControllers();
   
    builder.Services.Configure<IISServerOptions>(options =>
    {
        options.MaxRequestBodySize = int.MaxValue;
    });
    builder.Services.Configure<KestrelServerOptions>(options =>
    {
        options.Limits.MaxRequestBodySize = int.MaxValue;
    });
    builder.Services.Configure<FormOptions>(options =>
    {
        options.KeyLengthLimit = 8192; // Default is 2048
        options.ValueLengthLimit = 1024 * 1024 * 100; // Also increase value limit
        options.MultipartBodyLengthLimit = 1024 * 1024 * 100; // 100MB
    });
    builder.Services.AddScoped<AnnouncementRepository>();
    builder.Services.AddTransient<WhatsAppMessageService>();
    builder.Services.AddAntiforgery(options =>
    {
        options.HeaderName = "X-CSRF-TOKEN"; // You can choose a different header if you prefer
    });

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
     .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });


    //
    builder.Services.AddUserActivity(options =>
    {
        options.ConnectionString = userActivityConnectionstr;
        options.LogRequestBody = true;
        options.LogQueryString = true;
        options.MaxRequestBodyLength = 1000;
        options.ExcludePaths = new[] { "/health", "/metrics" };
        options.ExcludeContentTypes = new[] { "application/octet-stream" };
    });
    builder.Services.AddScoped<IUserActivityService, UserActivityService>();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.Configure<CaptchaConfiguration>
        (builder.Configuration.GetSection("CaptchaConfiguration"));
    // Add AutoMapper  
    builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

    // Add Repositories
    builder.Services.AddScoped<IApplicantRepository, ApplicantRepository>();
    builder.Services.AddScoped<IBankDetailsRepository, BankDetailsRepository>();
    builder.Services.AddScoped<IDependentRepository, DependentRepository>();

    // Add Services
    builder.Services.AddScoped<IApplicantService, ApplicantService>();
    builder.Services.AddScoped<IBankDetailsService, BankDetailsService>();
    builder.Services.AddScoped<IDependentService, DependentService>();
    // Add this to the service registration section in Program.cs
    builder.Services.AddScoped<IImportExportService, ImportExportService>();

    builder.Services.AddOptions<KMDAPISecretKey>().BindConfiguration(nameof(KMDAPISecretKey));
    builder.Services.AddOptions<SMSRequestObject>().BindConfiguration(nameof(SMSRequestObject));
    builder.Services.AddOptions<OTPConfiguration>().BindConfiguration(nameof(OTPConfiguration));
    builder.Services.AddOptions<GoDigitPaymentGateway>().BindConfiguration(nameof(GoDigitPaymentGateway));
    builder.Services.AddOptions<WhatsappRequestObject>().BindConfiguration(nameof(WhatsappRequestObject));
    builder.Services.AddOptions<WhatsappRequestObject>().BindConfiguration(nameof(WhatsappRequestObject));

    // Register as singleton in Program.cs
    builder.Services.AddSingleton<IVerificationService, VerificationService>();
    builder.Services.AddHostedService(sp => (VerificationService)sp.GetRequiredService<IVerificationService>());

    //builder.Services.Configure<SMSRequestObject>(builder.Configuration.GetSection("SMSRequestObject"));

    builder.Services.AddDbContext<UserActivityDbContext>(options =>
    options.UseSqlServer(userActivityConnectionstr));
    // Add DB Contexts
    // Move the connection string to user secrets for a real app
    builder.Services.AddDbContext<ApplicationDbContext>
        (opt => opt.UseSqlServer(connstr
        , b => b.MigrationsAssembly("KMDRecociliationApp.Domain")));

    builder.Services.AddHealthChecks()
           .AddDbContextCheck<ApplicationDbContext>() // Add DB context health check
           .AddSqlServer(
               connectionString: connstr,
               name: "sql_server_health",
               tags: new[] { "database" });
    builder.Services.AddControllers();

    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
    // Register the interceptor


    builder.Services.RegisterServices();
    //builder.Services.Configure<SMSRequestObject>(builder.Configuration.GetSection("SMSRequestObject"));
    builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

   
    // Register the Swagger generator, defining 1 or more Swagger documents
    builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    // To enable authorization in Swagger (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }});
});
    builder.Services.AddHealthChecks()
        .AddSqlServer(connstr);


    var app = builder.Build();
    app.UseRateLimiter();
    app.UseDefaultFiles();
    app.UseStaticFiles();
    //app.UseStaticFiles(new StaticFileOptions
    //{
    //    FileProvider = new PhysicalFileProvider(
    //       Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    //    RequestPath = "/Uploads"
    //});
    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
    if (!Directory.Exists(uploadPath))
    {
        Directory.CreateDirectory(uploadPath);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadPath),
        RequestPath = "/uploads"
    });

    // Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseRouting();
    app.UseCors("AllowOrigin");
   
    app.UseAuthentication();
    app.UseUserActivity();
    app.UseAuthorization();


    app.MapControllers();
    // Map health check endpoints
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                Status = report.Status.ToString(),
                Duration = report.TotalDuration,
                Info = report.Entries.Select(e => new
                {
                    Key = e.Key,
                    Status = e.Value.Status.ToString(),
                    Description = e.Value.Description,
                    Duration = e.Value.Duration,
                    Data = e.Value.Data
                })
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    });

    // Custom endpoint for detailed database health
    app.MapHealthChecks("/health/database", new HealthCheckOptions
    {
        Predicate = (check) => check.Tags.Contains("database"),
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                Status = report.Status.ToString(),
                Duration = report.TotalDuration,
                Info = report.Entries.Select(e => new
                {
                    Key = e.Key,
                    Status = e.Value.Status.ToString(),
                    Duration = e.Value.Duration,
                    Exception = e.Value.Exception?.Message
                })
            });
        }
    });

   
    
   

    app.MapFallbackToFile("/index.html");

    app.Run();
    return 0;
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Host terminated unexpectedly.");
    return 1;
}
finally
{
    await Log.CloseAndFlushAsync();
}
