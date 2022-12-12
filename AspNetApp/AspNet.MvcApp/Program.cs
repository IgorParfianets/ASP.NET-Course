using AspNetArticle.Business.Models;
using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Data.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using AspNetArticle.MvcApp.Filters;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using NuGet.Packaging.Core;
using Serilog;
using Serilog.Events;
using System.Configuration;

namespace AspNet.MvcApp
{
    public class Program 
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // For Logger Serilog 
            builder.Host.UseSerilog((ctx, lc) =>
                lc.WriteTo.File(
                        @"C:\Users\Igor\Desktop\data.log",
                        LogEventLevel.Information)
                    .WriteTo.Console(LogEventLevel.Information));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //------------------------------------------------------------

            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ISourceService, SourceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ICommentaryService, CommentaryService>();
            builder.Services.AddScoped<IArticleRateService, ArticleRateService>();
            builder.Services.AddScoped<ISendMessageService, SendMessageService>();
            builder.Services.AddScoped<IFavouriteArticleService, FavouriteArticleService>();

            builder.Services.AddScoped<IExtendedArticleRepository, ExtendedArticleRepository>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Source>, Repository<Source>>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
            builder.Services.AddScoped<IRepository<FavouriteArticle>, Repository<FavouriteArticle>>();

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

            //MailKit
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            //builder.Services.AddMailKit(optionsBuilder =>
            //    {
            //        var mailKitOptions = new MailKitOptions()
            //        {
            //            Server = builder.Configuration.GetValue<string>("MailSettings:Mail"),
            //            SenderName = builder.Configuration.GetValue<string>("MailSettings:DisplayName"),
            //            Password = builder.Configuration.GetValue<string>("MailSettings:Password"),
            //            SenderEmail = builder.Configuration.GetValue<string>("MailSettings:Host"),
            //            Port = builder.Configuration.GetValue<int>("MailSettings:Port"),
            //        };
            //        optionsBuilder.UseMailKit(mailKitOptions);
            //    }
            //);

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
            app.MapHangfireDashboard("/jobs", options: new DashboardOptions()
            {
                Authorization = new[] { new HangfireDashboardAuthorizationFilter() }
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}