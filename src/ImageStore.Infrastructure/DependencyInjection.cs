using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ImageStore.Domain.Interfaces;
using ImageStore.Infrastructure.Database.Repositories;
using Amazon.S3;

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
                //options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddAWSService<IAmazonS3>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IImageStorage, S3ImageStore>();

            return services;
        }
    }
}
