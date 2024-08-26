using API.BackgroundTasks;
using Application.CustomAutoMapper;
using Application.Interfaces;
using Application.Repository;
using Application.Services.TokenServices;
using Application.Services.WebInterfaces;
using Application.Services.WebServices;
using AutoMapper;
using Infrastructure.DBContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using NLog.Web;
using System.Reflection;
using System.Text;

var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    #region DATABASE
    ConfigurationManager configuration = builder.Configuration;
    var connectionString = configuration.GetConnectionString("SAMPLEDB");
    builder.Services.AddDbContext<SampleDBContext>(options => options.UseNpgsql(connectionString));
    builder.Services.AddDbContext<SampleReadOnlyDBContext>(options => options.UseNpgsql(connectionString));
    #endregion

    #region AUTOMAPPER
    // Add services to the container.
    builder.Services.AddAutoMapper(typeof(Program));

    var mappingConfig = new MapperConfiguration(mc =>
    {
        mc.AddProfile(new DomainToDTOMappingProfile());
    });
    IMapper mapper = mappingConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    #endregion
    builder.Services.AddControllers().AddNewtonsoftJson();
    builder.Services.AddControllersWithViews();
    #region BACKGROUND SERVICES
    // Service Background
    var on_off_BackgroundService = builder.Configuration.GetSection("BackgroundService").Value;
    if (on_off_BackgroundService == "true")
    {
        builder.Services.AddHostedService<ExampleBackground>();
    }


    //var appConfig = builder.Configuration.GetSection("Path").Get<ApplicationConfiguration>();
    //builder.Services.AddSingleton<IApplicationConfiguration, ApplicationConfiguration>(e => appConfig);
    #endregion
    #region AUITHEN, SESSION + JWT and Token
    //var tokenSecretKey = Utils.DecodePassword(builder.Configuration.GetSection("Path:JWT:SecretKey").Value, EEncodeType.SHA_256);
    var tokenSettings = builder.Configuration.GetSection("ExternalServices:TokenSettings");
    builder.Services.Configure<TokenSettings>(tokenSettings);
    var tokenSettingModel = tokenSettings.Get<TokenSettings>(); 
    builder.Services.AddAuthentication(cfg =>
    {
        cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //cfg.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.Response.OnStarting(async () =>
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
                            await context.Response.WriteAsync(result);
                        });

                        return Task.CompletedTask;
                    }
                };
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSettingModel.SecretKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });

    builder.Services.AddSession(options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(100);
    });
    #endregion


    builder.Services.AddSignalR(hubOptions =>
    {
        hubOptions.EnableDetailedErrors = true;
        hubOptions.ClientTimeoutInterval = TimeSpan.FromHours(24);
        hubOptions.KeepAliveInterval = TimeSpan.FromHours(8);
    });
    builder.Services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "JWTToken_Auth_API",
            Version = "v1"
        });
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
    });
    #region NLog: Setup NLog for Dependency injection
    logger.Debug("Running...");
    builder.Logging.ClearProviders();
    builder.Logging.AddDebug();
    builder.Logging.AddConsole();
    builder.Logging.SetMinimumLevel(LogLevel.Trace);
    builder.Host.UseNLog();
    #endregion

    #region Register Repository And UnitOfWork Settings

    // REGISTER SERVICES HERE
    builder.Services.AddScoped<ITokenService, TokenService>();

    //builder.Services.AddSingleton<IChatHub, ChatHub>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IRoleService, RoleService>();


    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRoleRepository, RoleRepository>();

    #endregion



    // REGISTER MIDDLEWARE HERE

    #region APP RUN
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

    app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    app.MapHub<ChatHub>("/chatHub");

    app.Run();
    #endregion
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
