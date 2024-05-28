using ImageStore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ImageStore.Domain.Interfaces;
using ImageStore.Infrastructure.ImageStore;

namespace ImageStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                // TODO: Use interceptor to save createdat and updatedat fields
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IImageStorage, S3ImageStore>();
            return services;
        }
    }
}
