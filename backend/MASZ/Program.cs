using AspNetCoreRateLimit;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MASZ.Data;
using MASZ.Logger;
using MASZ.Middlewares;
using MASZ.Plugins;
using MASZ.Repositories;
using MASZ.Services;
using MASZ.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Logging.AddProvider(new LoggerProvider());

builder.WebHost.UseUrls("http://0.0.0.0:80/");

string connectionString =
            $"Server={   Environment.GetEnvironmentVariable("MYSQL_HOST")};" +
            $"Port={     Environment.GetEnvironmentVariable("MYSQL_PORT")};" +
            $"Database={ Environment.GetEnvironmentVariable("MYSQL_DATABASE")};" +
            $"Uid={      Environment.GetEnvironmentVariable("MYSQL_USER")};" +
            $"Pwd={      Environment.GetEnvironmentVariable("MYSQL_PASSWORD")};";

if (false) {
    connectionString =
            $"Server=localhost;" +
            $"Port=3306;" +
            $"Database=masz;" +
            $"Uid=root;" +
            $"Pwd=root;";
}

builder.Services.AddDbContext<DataContext>(x => x.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddControllers()
    .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services

// SINGLETONS

.AddSingleton(new DiscordSocketConfig
{
    AlwaysDownloadUsers = true,
    MessageCacheSize = 10240,
    LogLevel = LogSeverity.Debug,
    GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
    LogGatewayIntentWarnings = false
})

.AddSingleton<DiscordSocketClient>()

.AddSingleton(new InteractionServiceConfig
{
    DefaultRunMode = RunMode.Async,
    LogLevel = LogSeverity.Debug,
    UseCompiledLambda = true
})

.AddSingleton<InteractionService>()

.AddSingleton<FilesHandler>()

.AddSingleton<InternalConfiguration>()

.AddSingleton<DiscordAPIInterface>()

.AddSingleton<IdentityManager>()

.AddSingleton<InternalEventHandler>()

.AddSingleton<AuditLogger>()

.AddSingleton<DiscordBot>()

.AddSingleton<Punishments>()

.AddSingleton<Scheduler>()

.AddSingleton<DiscordAnnouncer>()

.AddSingleton<GuildAuditLogger>()

// SCOPED

.AddScoped<Database>()

.AddScoped<Translator>();

// Plugin
// ######################################################################################################

if (string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS")))
{
    Console.WriteLine("########################################################################################################");
    Console.WriteLine("ENABLED CUSTOM PLUGINS!");
    Console.WriteLine("This might impact the performance or security of your MASZ instance!");
    Console.WriteLine("Use this only if you know what you are doing!");
    Console.WriteLine("For support and more information, refer to the creator or community of your plugin!");
    Console.WriteLine("########################################################################################################");

    builder.Services.Scan(scan => scan
        .FromAssemblyOf<IBasePlugin>()
        .AddClasses(classes => classes.InNamespaces("MASZ.Plugins"))
        .AsImplementedInterfaces()
        .WithSingletonLifetime());
}
// ######################################################################################################

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/api/v1/login";
        options.LogoutPath = "/api/v1/logout";
        options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
        options.Cookie.MaxAge = new TimeSpan(7, 0, 0, 0);
        options.Cookie.Name = "masz_access_token";
        options.Cookie.HttpOnly = false;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.Headers["Location"] = context.RedirectUri;
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    })
    .AddDiscord(options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("DISCORD_OAUTH_CLIENT_ID");
        options.ClientSecret = Environment.GetEnvironmentVariable("DISCORD_OAUTH_CLIENT_SECRET");
        options.Scope.Add("guilds");
        options.Scope.Add("identify");
        options.SaveTokens = true;
        options.Prompt = "none";
        options.AccessDeniedPath = "/oauthfailed";
        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.CorrelationCookie.SameSite = SameSiteMode.Lax;
        options.CorrelationCookie.HttpOnly = false;
    });


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Tokens", x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"))),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder("Cookies", "Tokens")
        .RequireAuthenticatedUser()
        .Build();
});

if (string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CORS")))
{
    builder.Services.AddCors(o => o.AddPolicy("AngularDevCors", builder =>
    {
        builder.WithOrigins("http://127.0.0.1:4200", "http://127.0.0.1:8080", "http://127.0.0.1:5500")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    }));
}

// Needed to store rate limit counters and ip rules
builder.Services.AddMemoryCache();

// Load general configuration from appsettings.json
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

// Load ip rules from appsettings.json
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

builder.Services

.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>()

.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>()

.AddSingleton<IHttpContextAccessor, HttpContextAccessor>()

.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>()

.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

builder.Services.AddMvc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<HeaderMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<APIExceptionHandlingMiddleware>();

if (string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CORS")))
{
    app.UseCors("AngularDevCors");
}

app.UseIpRateLimiting();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<DataContext>().Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<InternalConfiguration>().Init();

    await AppSettingsRepository.CreateDefault(scope.ServiceProvider).ApplyAppSettings();

    scope.ServiceProvider.GetRequiredService<AuditLogger>().RegisterEvents();
    scope.ServiceProvider.GetRequiredService<DiscordAnnouncer>().RegisterEvents();
    scope.ServiceProvider.GetRequiredService<DiscordBot>().RegisterEvents();
    scope.ServiceProvider.GetRequiredService<GuildAuditLogger>().RegisterEvents();
    scope.ServiceProvider.GetRequiredService<Scheduler>().RegisterEvents();

    await scope.ServiceProvider.GetRequiredService<AuditLogger>().ExecuteAsync();
    await scope.ServiceProvider.GetRequiredService<DiscordBot>().ExecuteAsync();

    if (string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS")))
        scope.ServiceProvider.GetServices<IBasePlugin>().ToList().ForEach(x => x.Init());
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

await app.RunAsync();