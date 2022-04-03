using ADOServices.ADOServices.Jobs;
using ADOServices.ADOServices.OTPServices;
using ADOServices.ADOServices.RoleServices;
using ADOServices.ADOServices.UserServices;
using Database;
using Database.ADO;
using Database.Repository;
using GlobalExceptionHandling.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.ADOServices.Jobs;
using Services.ADOServices.RoleServices;
using Services.Jobs;
using Services.OTPService;
using Services.Roles;
using Services.UserServices;
using System;
using System.Net;
using System.Text;

namespace API
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
            services.AddMemoryCache();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddDbContext<DbContextModel>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("Connection")));

            services
                //.AddScoped<IDB, DB>()

                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserService, UserService>()

                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IRoleService, RoleService>()

                .AddScoped<IAppliedJobsRepository, AppliedJobRepository>()
                .AddScoped<IAppliedJobsService, AppliedJobsService>()

                .AddScoped<IJobsRepository, JobsRepository>()
                .AddScoped<IJobService, JobService>()

                .AddScoped<IOTPRepository, OTPRepository>()
                .AddScoped<IOTPService, OTPService>()


                .AddScoped<IAppliedJobsServices, AppliedJobsServices>()
                .AddScoped<IJobsServices, JobsServices>()
                .AddScoped<IOTPServices, OTPServices>()
                .AddScoped<IRoleServices, RolesServices>()
                .AddScoped<IUserServices, UserServices>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Recruiter", policy => policy.RequireRole("Admin", "Recruiter"));
                options.AddPolicy("AllAllowed", policy => policy.RequireRole("Admin", "Recruiter", "User"));
                options.AddPolicy("User", policy => policy.RequireRole("Admin", "User"));
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

           .AddJwtBearer(options =>
           {
               options.SaveToken = true;
               options.RequireHttpsMetadata = false;
               options.TokenValidationParameters = new TokenValidationParameters()
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.Zero,
                   ValidAudience = Configuration["JWT:ValidAudience"],
                   ValidIssuer = Configuration["JWT:ValidIssuer"],
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
               };
           });

            services.AddControllers();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddControllersWithViews()
            .AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(options => options.AllowAnyOrigin()
                                          .AllowAnyHeader()
                                          .AllowAnyMethod());

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseExceptionHandler(
            //     options =>
            //    {
            //        options.Run(async context =>
            //        {
            //            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            //            context.Response.ContentType = "text/html";
            //            var exceptionObject = context.Features.Get<IExceptionHandlerFeature>();
            //            if (null != exceptionObject)
            //            {
            //                var errorMessage = $"{exceptionObject.Error.Message}";
            //                await context.Response.WriteAsync(errorMessage).ConfigureAwait(false);
            //            }
            //        });
            //    });

        }
    }
}
