using AspNetCoreRateLimit;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using MASZ.Data;
using MASZ.Logger;
using MASZ.Middlewares;
using MASZ.Plugins;
using MASZ.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MASZ
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x.UseMySql($"Server=" + Environment.GetEnvironmentVariable("MYSQL_HOST") + ";" +
                                                               $"Port=" + Environment.GetEnvironmentVariable("MYSQL_PORT") + ";" +
                                                               $"Database=" + Environment.GetEnvironmentVariable("MYSQL_DATABASE") + ";" +
                                                               $"Uid=" + Environment.GetEnvironmentVariable("MYSQL_USER") + ";" +
                                                               $"Pwd=" + Environment.GetEnvironmentVariable("MYSQL_PASSWORD") + ";"));
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            int shards;

            using (var restClient = new DiscordRestClient())
            {
                await restClient.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));

                shards = await restClient.GetRecommendedShardCountAsync();
            }

            services.AddSingleton(provider =>
             {
                 var client = new DiscordShardedClient(new DiscordSocketConfig
                 {
                     AlwaysDownloadUsers = true,
                     MessageCacheSize = 50,
                     TotalShards = shards,
                     LogLevel = LogSeverity.Debug
                 });

                 return client;
             })

            .AddSingleton(new InteractionServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug,
                UseCompiledLambda = true
            })

            .AddSingleton<InteractionService>();

            services.AddScoped<IDatabase, Database>();
            services.AddScoped<ITranslator, Translator>();
            services.AddScoped<INotificationEmbedCreator, NotificationEmbedCreator>();
            services.AddScoped<IDiscordAnnouncer, DiscordAnnouncer>();
            services.AddSingleton<IFilesHandler, FilesHandler>();
            services.AddSingleton<IInternalConfiguration, InternalConfiguration>();
            services.AddSingleton<IDiscordBot, DiscordBot>();
            services.AddSingleton<IDiscordAPIInterface, DiscordAPIInterface>();
            services.AddSingleton<IIdentityManager, IdentityManager>();
            services.AddSingleton<IPunishmentHandler, PunishmentHandler>();
            services.AddSingleton<IEventHandler, Services.EventHandler>();
            services.AddSingleton<IScheduler, Scheduler>();
            services.AddSingleton<IAuditLogger, AuditLogger>();

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

                services.Scan(scan => scan
                    .FromAssemblyOf<IBasePlugin>()
                    .AddClasses(classes => classes.InNamespaces("MASZ.Plugins"))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime());
            }
            // ######################################################################################################

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/api/v1/login";
                    options.LogoutPath = "/api/v1/logout";
                    options.ExpireTimeSpan = new TimeSpan(7, 0, 0, 0);
                    options.Cookie.MaxAge = new TimeSpan(7, 0, 0, 0);
                    options.Cookie.Name = "MASZ_access_token";
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


            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

            services.AddAuthorization(options =>
                {
                    options.DefaultPolicy = new AuthorizationPolicyBuilder("Cookies", "Tokens")
                        .RequireAuthenticatedUser()
                        .Build();
                });

            if (string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CORS")))
            {
                services.AddCors(o => o.AddPolicy("AngularDevCors", builder =>
                {
                    builder.WithOrigins("http://127.0.0.1:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                }));
            }

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddMvc();

            // https://github.com/aspnet/Hosting/issues/793
            // the IHttpContextAccessor service is not registered by default.
            // the clientId/clientIp resolvers use it.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // configuration (resolvers, counter key builders)
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new CustomLoggerProvider());

            if (env.IsDevelopment())
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

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<DataContext>().Database.Migrate();
            }

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                scope.ServiceProvider.GetService<IInternalConfiguration>().Init();
                scope.ServiceProvider.GetService<IAuditLogger>().Startup();
                scope.ServiceProvider.GetService<IPunishmentHandler>().StartTimer();
                scope.ServiceProvider.GetService<IScheduler>().StartTimers();
                scope.ServiceProvider.GetService<IDiscordBot>().Start();
                if (string.Equals("true", Environment.GetEnvironmentVariable("ENABLE_CUSTOM_PLUGINS")))
                {
                    scope.ServiceProvider.GetServices<IBasePlugin>().ToList().ForEach(x => x.Init());
                }
                scope.ServiceProvider.GetService<IAuditLogger>().RegisterEvents();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
