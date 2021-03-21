using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

            /*
            this method was added as an extension method for maintainability of the code, check previous commit to understand
            how it was before.
            This method is used to implement dependency injection and also to specify 
            the life cycle of the dependency
            we have 3 methods to add a service
            1. AddSingleton (once created it will not dispose of that object till the application shuts down)
            2. AddScoped this life cycle is scoped as long as the http request is processed which is ideally suitable
            3. AddTransient to be used when services are created and destroyed as soon as the method is finished executing
            
            we will be using AddScoped
            also note will be injecting the interface along with class, as its a better practice and good for testability

            */
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(config.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}