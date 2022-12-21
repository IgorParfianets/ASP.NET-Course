using AspNetArticle.Api.Utils;
using AspNetArticle.Business.Services;
using AspNetArticle.Core.Abstractions;
using AspNetArticle.Data.Abstractions.Repositories;
using AspNetArticle.Data.Abstractions;
using AspNetArticle.Data.Repositories;
using AspNetArticle.Database;
using AspNetArticle.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hangfire;
using Hangfire.SqlServer;
using AspNetArticle.Business.Models;
using AsoNetArticle.Data.CQS.Commands;
using AsoNetArticle.Data.CQS.Handers;
using AsoNetArticle.Data.CQS.Queries;
using MediatR;
using AspNetSample.Data.CQS.Commands;
using AsoNetArticle.Data.CQS.Handers.QueryHanders;

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
            builder.Services.AddScoped<IFavouriteArticleService, FavouriteArticleService>();

            builder.Services.AddScoped<IExtendedArticleRepository, ExtendedArticleRepository>();
            builder.Services.AddScoped<IRepository<User>, Repository<User>>();
            builder.Services.AddScoped<IRepository<Role>, Repository<Role>>();
            builder.Services.AddScoped<IRepository<Source>, Repository<Source>>();
            builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
            builder.Services.AddScoped<IRepository<FavouriteArticle>, Repository<FavouriteArticle>>();

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

            //MediatR
            builder.Services.AddMediatR(typeof(GetArticleByIdQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetAllArticlesQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetArticlesFilteredQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetArticleCategoriesQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetArticleIdByCommentIdQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetAllSourcesQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetArticlesIdWithEmptyTextQuery).Assembly);
            builder.Services.AddMediatR(typeof(AddArticleDataFromRssFeedCommand).Assembly);
            builder.Services.AddMediatR(typeof(UpdateArticleOnlinerCommand).Assembly);
            builder.Services.AddMediatR(typeof(UpdateArticleDevIoCommand).Assembly);

            builder.Services.AddMediatR(typeof(GetArticlesIdWithEmptyRateQuery).Assembly);
            builder.Services.AddMediatR(typeof(UpdateArticleRateCommand).Assembly);
            builder.Services.AddMediatR(typeof(GetCommentByIdQuery).Assembly);
            builder.Services.AddMediatR(typeof(AddCommentCommand).Assembly);
            builder.Services.AddMediatR(typeof(UpdateCommentCommand).Assembly);
            builder.Services.AddMediatR(typeof(GetAllCommentsByUserIdQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetAllCommentsQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetAllCommentsWithUsersByArticleIdQuery).Assembly);
            builder.Services.AddMediatR(typeof(DeleteCommentByIdCommand).Assembly);

            builder.Services.AddMediatR(typeof(AddFavouriteArticleCommand).Assembly);
            builder.Services.AddMediatR(typeof(GetFavouriteArticleByUserIdAndArticleIdQuery).Assembly);
            builder.Services.AddMediatR(typeof(DeleteFavouriteArticleCommand).Assembly);
            builder.Services.AddMediatR(typeof(GetAllFavouriteArticlesIdByUserIdQuery).Assembly);

            builder.Services.AddMediatR(typeof(GetRoleIdByNameQuery).Assembly);

            builder.Services.AddMediatR(typeof(GetUserEmailsForSendSpamQuery).Assembly);
            builder.Services.AddMediatR(typeof(AddUserCommand).Assembly);
            builder.Services.AddMediatR(typeof(GetUserByIdQuery).Assembly);
            builder.Services.AddMediatR(typeof(UpdateUserCommand).Assembly);
            builder.Services.AddMediatR(typeof(GetUserByEmailQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetUserByUsernameQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetAllUsersQuery).Assembly);

            builder.Services.AddMediatR(typeof(AddRefreshTokenCommand).Assembly);
            builder.Services.AddMediatR(typeof(DeleteRefreshTokenCommand).Assembly);
            builder.Services.AddMediatR(typeof(GetUserByRefreshTokenQuery).Assembly);
            builder.Services.AddMediatR(typeof(GetCommentsByUserIdAndArticleIdQuery).Assembly);
            

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