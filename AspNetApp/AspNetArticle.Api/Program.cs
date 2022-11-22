using AspNetArticle.Api.Utils;
using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using AspNetArticle.Business.Models;

namespace AspNetArticle.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Host.UseSerilog((ctx, lc) =>
                lc.WriteTo.File(
                        @"C:\Users\Igor\Desktop\data.log",
                        LogEventLevel.Information)
                    .WriteTo.Console(LogEventLevel.Verbose));
            //Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<ISourceService, SourceService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IRoleService, RoleService>();
            builder.Services.AddScoped<ICommentaryService, CommentaryService>();
            builder.Services.AddScoped<IArticleRateService, ArticleRateService>();
            builder.Services.AddScoped<ISendMessageService, SendMessageService>();

            builder.Services.AddScoped<IExtendedArticleRepository, ExtendedArticleRepository>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Source>, Repository<Source>>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();

            builder.Services.AddScoped<IJwtUtil, JwtUtil>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(builder.Configuration["XmlDoc"]);
            });

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

            builder.Services.AddHangfireServer();
            // Mapper
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // For Mapping to collect all profiles

            //SendMessage
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.SaveToken = true;
                    opt.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = builder.Configuration["Token:Issuer"],
                        ValidAudience = builder.Configuration["Token:Issuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:JwtSecret"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            // Configuration
            //builder.Configuration.AddJsonFile("hashingsalt.json"); //todo remove for custom configuration
            var app = builder.Build();

            app.UseStaticFiles();
            app.UseHangfireDashboard();
            app.UseRouting();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapHangfireDashboard();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
} // todo add XmlLog