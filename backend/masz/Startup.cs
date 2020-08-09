using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using masz.data;
using masz.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace masz
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();

            services.AddScoped<IAuthRepository, AuthRepository>();

             services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })

            .AddCookie(options =>
            {
                options.LoginPath = "/api/v1/login";
                options.LogoutPath = "/api/v1/logout";
                options.ExpireTimeSpan = new TimeSpan(24, 0, 0);
                options.Cookie.Name = "access_token";
            })
            
            .AddDiscord(options =>
            {
                options.ClientId = Configuration.GetSection("InternalConfig").GetValue<string>("DiscordClientId");
                options.ClientSecret = Configuration.GetSection("InternalConfig").GetValue<string>("DiscordClientSecret");
                options.Scope.Add("guilds");
                options.Scope.Add("identify");
                options.SaveTokens = true;
                options.Prompt = Discord.OAuth2.DiscordOptions.PromptTypes.None;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.Configure<InternalConfig>(Configuration.GetSection("InternalConfig"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
