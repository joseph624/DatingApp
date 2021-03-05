using API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using API.Services;
using API.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Extensions;
using API.Middleware;
using API.SignalR;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationServices(_config);
            services.AddControllers();
            services.AddCors();
            services.AddIdentityServices(_config);
            services.AddSignalR();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Exception Handling
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(policy => policy.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials() // for signalR
                    .WithOrigins("https://localhost:4200"));

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseDefaultFiles(); // for production of static files
            app.UseStaticFiles(); // for production of static fileds

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PresenceHub>("hubs/presence"); // route this presence hub will be accessed from
                endpoints.MapHub<MessageHub>("hubs/message"); // route this message hub will be accessed from
                endpoints.MapFallbackToController("Index", "Fallback"); // endpoint for route fallback
            });
        }
    }
}
