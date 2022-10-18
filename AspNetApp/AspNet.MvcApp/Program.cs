using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Data.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Core;

namespace AspNet.MvcApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //------------------------------------------------------------
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IRepository<Article>, Repository<Article>>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //------------------------------------------------------------

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            builder.Services.AddDbContext<AggregatorContext>(optionBuilder =>
            optionBuilder.UseSqlServer(builder.Configuration.GetConnectionString("Default"))); // For DB (connectionString in config files)

            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // For Mapping to collect all profiles

            builder.Configuration.AddJsonFile("passwordSalt.json"); // for custom configuration

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Test}/{action=MyIndex}/{id?}");

            app.Run();
        }
    }
}