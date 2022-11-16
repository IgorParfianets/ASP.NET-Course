using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Data.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Core;
using Serilog;
using Serilog.Events;

namespace AspNet.MvcApp
{
    public class Program //todo in Automapper to fix problem with Models
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // For Logger Serilog !!!
            builder.Host.UseSerilog((ctx, lc) =>
                lc.WriteTo.File(
                        @"C:\Users\Igor\Desktop\data.log",
                        LogEventLevel.Information)
                    .WriteTo.Console(LogEventLevel.Verbose));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //------------------------------------------------------------

            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ISourceService, SourceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ICommentaryService, CommentaryService>();
            builder.Services.AddScoped<IArticleRateService, ArticleRateService>();

            builder.Services.AddScoped<IExtendedArticleRepository, ExtendedArticleRepository>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Source>, Repository<Source>>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //------------------------------------------------------------

            // Authentication configuration
            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = new PathString(@"/Account/Login");
                    options.LogoutPath = new PathString(@"/Account/Logout");
                    options.AccessDeniedPath = new PathString(@"/Account/Login");
                });

            builder.Services.AddAuthorization(); // Test Remove Or Not

            // Db Context
            var connectionString = builder.Configuration.GetConnectionString("Default");
            builder.Services.AddDbContext<AggregatorContext>(optionBuilder =>
            optionBuilder.UseSqlServer(connectionString)); // For DB (connectionString in config files)

            //Hangfire config
            builder.Services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true,
                    }));
            // Mapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // For Mapping to collect all profiles

            // Configuration
            //builder.Configuration.AddJsonFile("hashingsalt.json"); //todo remove!!! for custom configuration 

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseHangfireDashboard();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseSession(); // Check that is mean
            app.MapHangfireDashboard("/jobs");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        /*
         * todo add Authenticate => Claims => Roles
         * todo add Identity? (SignInManager and UserManager)
         * todo implement Edit for user ( private room ) => add Image/Ava
         * todo refactor names / ASYNC / try-catch in controllers
         * todo edit entities some add/some remove
         *
         * todo implement Logger Serilog
         *
         *
         *
         * todo Fix Onliner news remove incut + reduce images in all news
         * todo watch lecture JS Web and make something with Authentication
         */
    }
}