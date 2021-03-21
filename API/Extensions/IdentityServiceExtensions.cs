using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            /*
               This method was added as an extension method for code maintainability as it keeps growing during the tutorial of the applications
               because we will include other logics w.r.t authentication and authorization.
               check previous commits to see how it was used before.
               In order to utilize the functionality of Authorize Decorator we need to install a
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
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            //Below parameter tells the app that it should be signing the token key hence true
                            ValidateIssuerSigningKey = true,
                            //Below parameter obtains the key thats stored in the server
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),
                            //Below paramter specifies if the app needs to validate the token issuer, since the 
                            //app itself is issuing the token its set to false for now
                            //issuer is our api server
                            ValidateIssuer = false,
                            //this is the parameter for the audience in this context our angular application
                            ValidateAudience = false

                        };
                    });

            return services;
        }
    }
}