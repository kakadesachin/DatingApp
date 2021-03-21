using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;

        }



        // This method gets called by the runtime. Use this method to add services to the container.
        // This can also be referred as a dependency injection container.
        public void ConfigureServices(IServiceCollection services)
        {
            /*
            This method is used to implement dependency injection and also to specify 
            the life cycle of the dependency
            we have 3 methods to add a service
            1. AddSingleton (once created it will not dispose of that object till the application shuts down)
            2. AddScoped this life cycle is scoped as long as the http request is processed which is ideally suitable
            3. AddTransient to be used when services are created and destroyed as soon as the method is finished executing
            
            we will be using AddScoped
            also note will be injecting the interface along with class, as its a better practice and good for testability

            */
            services.AddScoped<ITokenService,TokenService>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(_config.GetConnectionString("DefaultConnection"));
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            });
            services.AddCors();

            /* In order to utilize the functionality of Authorize Decorator we need to install a
               middle ware called Microsoft.AspNetCore.Authentication.JwtBearer
               that will be used to authenticate users using jwt

               below code will do the same.
               by adding the below logic
               which ever action is decorated with Authroize tag
               will only execute if the requests header contains the Key
               which is 
               "Authorization":"Bearer {ourJWT}"
            */

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>{
                        options.TokenValidationParameters = new TokenValidationParameters{
                            //Below parameter tells the app that it should be signing the token key hence true
                            ValidateIssuerSigningKey = true,
                            //Below parameter obtains the key thats stored in the server
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"])),
                            //Below paramter specifies if the app needs to validate the token issuer, since the 
                            //app itself is issuing the token its set to false for now
                            //issuer is our api server
                            ValidateIssuer=false,
                            //this is the parameter for the audience in this context our angular application
                            ValidateAudience=false

                        };
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
            app.UseCors(policy=>policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
            //Note Authentication should come after UseCors
            //and Authorization comes after Authentication
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
