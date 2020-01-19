using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using GorgleDevs.Mvc;
using CcgWorks.MartenStore;
using CcgWorks.SimpleDbStore;


namespace CcgWorks.Api
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
            var awsOptions = Configuration.GetAWSOptions();

            services.AddCors();
            services.AddHttpContextAccessor();            
            services.AddScoped<IUserContext, ClaimsIdentityUserContext>();

            var env = Configuration.GetValue<string>("HOST") ?? "AWS";
            if(env == "local")
            {
                Console.WriteLine("Running locally");
                var dbHost = Configuration.GetValue<string>("DB_HOST") ?? "localhost";
                var dbPort = Configuration.GetValue<string>("DB_PORT") ?? "5432";
                var user = Configuration.GetValue<string>("DB_USER") ?? "postgres";
                var pwd = Configuration.GetValue<string>("DB_PWD") ?? "admin";
                services.AddMarten($"Server={dbHost};Port={dbPort};Database=ccgworks;User Id={user};Password={pwd};");
                services.AddLocalImageStore(@"wwwroot\images\store\", "http://localhost:5000/images/store");
            }
            else
            {                
                Console.WriteLine("Running in AWS");
                var bucketName = Configuration.GetValue<string>("IMAGE_STORE_BUCKET") ?? "ccgworks-images";
                var distributionUrl = Configuration.GetValue<string>("IMAGE_STORE_URL") ?? "https://dd1bt1ytlgdzr.cloudfront.net";
                services.AddSimpleDbStore(awsOptions);
                services.AddS3ImageStore(awsOptions, bucketName, distributionUrl);                
            }
                                    
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = Authentication.GetCognitoKey(awsOptions),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };
                });

            services.AddMvc(opts => 
            {
                opts.UseShortGuids();
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.Map(
                    new PathString("/health-check"),
                    a => a.Use(async (context, next) =>
                    {
                        await context.Response.WriteAsync("ok");
                    }));
            }

            app.Map(new PathString("/info"),
                a => a.Use(async (context, next) => 
                {
                    await context.Response.WriteAsync("ccgworks:api-0.1");
                }));
            
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
        }                
    }
}
