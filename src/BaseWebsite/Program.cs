using Application.CustomAutoMapper;
using Application.Interfaces;
using Application.Repository;
using Application.Services.WebInterfaces;
using Application.Services.WebServices;
using AutoMapper;
using BaseWebsite.Authorizations;
using BaseWebsite.BackgroundServices;
using Core.CommonModels;
using Core.CoreUtils;
using Core.Enums;
using Infrastructure.DBContexts;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog.Web;
using System.Reflection;
using System.Text;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    var tokenSecretKey = Utils.DecodePassword(builder.Configuration.GetSection("JWT:SecretKey").Value, EEncodeType.SHA_256);

    ConfigurationManager configuration = builder.Configuration;

    // Add services to the container.
    builder.Services.AddAutoMapper(typeof(Program));

    var mappingConfig = new MapperConfiguration(mc =>
    {
        mc.AddProfile(new DomainToDTOMappingProfile());
    });
    IMapper mapper = mappingConfig.CreateMapper();

    builder.Services.AddSingleton(mapper);

    //builder.Services.AddSignalR();


    builder.Services.AddCors();

    builder.Services.AddControllers();
    builder.Services.AddControllersWithViews();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    var connectionString = configuration.GetConnectionString("QLLCDB");
    var CookieName_ = configuration.GetSection("Path:AuthCookie_").Value;
    builder.Services.AddDbContext<SampleDBContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddDbContext<SampleReadOnlyDBContext>(options => options.UseNpgsql(connectionString));

    //var appConfig = builder.Configuration.GetSection("Path").Get<ApplicationConfiguration>();
    //builder.Services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>(e => appConfig);
    #region SETTING AUTHEN & AUTHOR
    builder.Services.AddSingleton<IAuthorizationPolicyProvider, C3FunctionAuthorizationPolicyProvider>();
    builder.Services.AddSingleton<IAuthorizationHandler, C3FunctionAuthorizationHandler>();
    builder.Services.AddAuthentication(cfg =>
    {
        cfg.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        cfg.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;

    })
            .AddCookie(
                options =>
                {
                    options.Cookie.Name = CookieName_;
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    // cookie settings
                    options.Cookie.HttpOnly = true;
                    //options.Cookie.MaxAge = TimeSpan.FromSeconds(10);
                    //options.Cookie.Expiration = TimeSpan.FromSeconds(10);
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);


                })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json; charset=utf-8";
                        var result = JsonConvert.SerializeObject(
                            new
                            {
                                statusCode = 401,
                                message = "Token mismatch",
                                receivedRequest = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                sendResponse = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")
                            }
                        );
                        return context.Response.WriteAsync(result);
                    }
                };
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.FromSeconds(5)
                };
            });
    #endregion
    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(100);
    });
    builder.Services.AddRazorPages();
    //builder.Services.AddSignalR(hubOptions =>
    //{
    //    hubOptions.EnableDetailedErrors = true;
    //    hubOptions.ClientTimeoutInterval = TimeSpan.FromHours(24);
    //    hubOptions.KeepAliveInterval = TimeSpan.FromHours(8);
    //});
    builder.Services.AddSwaggerGen();
    #region NLog: Setup NLog for Dependency injection
    logger.Debug("Running...");
    builder.Logging.ClearProviders();
    builder.Logging.AddDebug();
    builder.Logging.AddConsole();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();
    #endregion

    #region ConfigurationRepositoryAndUnitOfWorkSettings
    // REGISTER SERVICES HERE
    //builder.Services.AddSingleton<IChatHub, ChatHub>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IRoleService, RoleService>();


    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();
    #endregion

    builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
    builder.Services.AddTransient<EmailService>();

    builder.Services.AddHttpClient<TelegramService>();
    builder.Services.AddHostedService<BotHostedBackgroundService>();



    // REGISTER MIDDLEWARE HERE
    var app = builder.Build();
    var environment = app.Environment;
    var config = builder.Configuration;
    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true);

    config.AddEnvironmentVariables().Build();
    if (environment.IsDevelopment())
    {
        var appAssembly = Assembly.Load(new AssemblyName(environment.ApplicationName));
        if (appAssembly != null)
            config.AddUserSecrets(appAssembly, optional: true);
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test1 Api v1");
        });
    }
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.MapSwagger().RequireAuthorization();

    app.UseCors(x => x
         .SetIsOriginAllowed(origin => true)
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials());

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();


    //app.UseEndpoints(endpoints =>
    //{
    //    endpoints.MapControllerRoute(
    //        name: "default",
    //        pattern: "{controller=Home}/{action=Index}/{id?}");

    //    //endpoints.MapHub<ChatHub>("/chatHub");
    //    endpoints.MapRazorPages();
    //});

    app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    //app.MapHub<ChatHub>("/chatHub");
    app.MapRazorPages();
    // Start the Telegram service
    //var telegramService = app.Services.GetRequiredService<TelegramService>();
    //telegramService.Start();
    app.Run();

}
catch (Exception ex)
{
    logger.Error(ex, "Error in init");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
