using Chess.Dal;
using Chess.Dal.Interfaces;
using Chess.Dal.Repositories;
using Chess.Models;
using Chess.Service.Implementations;
using Chess.Service.Interfaces;
using global::Microsoft.AspNetCore.Builder;
using global::Microsoft.AspNetCore.Hosting;
using global::Microsoft.AspNetCore.HttpsPolicy;
using global::Microsoft.EntityFrameworkCore;
using global::Microsoft.Extensions.Configuration;
using global::Microsoft.Extensions.DependencyInjection;
using global::Microsoft.Extensions.Hosting;
using global::System;
using global::System.Collections.Generic;
using global::System.Linq;
using global::System.Security.Claims;
using global::System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace Chess
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IGameService, GameService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IActiveRoomRepository, ActiveRoomRepository>();
            builder.Services.AddScoped<IActiveRoomService, ActiveRoomService>();

            builder.Services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true; 
                o.KeepAliveInterval = TimeSpan.FromSeconds(15);
                o.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                o.MaximumReceiveMessageSize = 64 * 1024;
            });

            builder.Services.AddDbContextFactory<ApplicationDBContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Default"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Default"))
    ));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
            {
                option.LoginPath = new PathString("/Login/Login");
                option.AccessDeniedPath = new PathString("/Login/Login");

            });

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".MyApp.Session";
                options.IdleTimeout = TimeSpan.FromHours(672);
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddHostedService<RoomCleanupService>();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapHub<GameHub>("/hubs/game");

            app.Run();
        }
    }
}
