using DataAccessObjects;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;

namespace ASM1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddCookie(options =>
   {
       options.LoginPath = "/Index";
       options.AccessDeniedPath = "/AccessDenied";
       options.Cookie.Name = "ASM01.AuthCookie";
       options.Cookie.HttpOnly = true;
       options.Cookie.SameSite = SameSiteMode.Lax;
   })
   .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
   {
       options.ClientId = "528609117986-m2vt7j95tsm2s3a2ecr37bgh0ietnv0f.apps.googleusercontent.com";
       options.ClientSecret = "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";
       options.CallbackPath = "/signin-google";
       options.SaveTokens = true;
       options.CorrelationCookie.SameSite = SameSiteMode.Lax;
   });

            builder.Services.AddAuthorization();
            builder.Services.AddSession();
            builder.Services.AddDbContext<FuNewsManagementContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                );

            builder.Services.AddHttpClient();

            //DI for CategoryRepository
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            //DI for NewsArticleRepository
            builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
            builder.Services.AddScoped<INewsArticleService, NewsArticleService>();

            //DI for SystemAccountRepository
            builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
            builder.Services.AddScoped<ISystemAccountService, SystemAccountService>();

            //DI for TagRepository
            builder.Services.AddScoped<ITagRepository, TagRepository>();
            builder.Services.AddScoped<ITagService, TagService>();

            //Session configuration
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Th?i gian timeout c?a session
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
