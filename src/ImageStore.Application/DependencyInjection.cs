using ImageStore.Application.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using ImageStore.Application.Configuration;
using Microsoft.Extensions.Configuration;

namespace ImageStore.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtConfiguration>(x => 
            {
                var jwtConfigurationSection = configuration.GetRequiredSection("Jwt");
                x.Issuer = jwtConfigurationSection.GetRequiredSection(nameof(JwtConfiguration.Issuer)).Value;
                x.Key = jwtConfigurationSection.GetRequiredSection(nameof(JwtConfiguration.Key)).Value;
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(config => {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            });
            return services;
        }
    }
}
