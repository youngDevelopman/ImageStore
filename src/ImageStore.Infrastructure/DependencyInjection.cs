using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ImageStore.Domain.Interfaces;
using ImageStore.Infrastructure.Database.Repositories;
using Amazon.S3;
using ImageStore.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ImageStore.Infrastructure.Database.Interceptors;

namespace ImageStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, AwsCredentialsConfuguration awsCredentialsConfuguration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddScoped<ISaveChangesInterceptor, UpdateTimestampsInterceptor>();
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                // TODO: Use interceptor to save createdat and updatedat fields
                options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
                options.UseSqlServer(connectionString);
            });

            services.Configure<S3Configuration>(x =>
            {
                var section = configuration.GetRequiredSection("AWS:S3Images");
                x.Bucket = section.GetRequiredSection(nameof(S3Configuration.Bucket)).Value;
                x.OriginalImageFolder = section.GetRequiredSection(nameof(S3Configuration.OriginalImageFolder)).Value;
            });

            //services.AddDefaultAWSOptions(configuration.GetAWSOptions()); Convenient, but performs badly :(

            services.AddScoped<IAmazonS3>(x => 
            {
                return new AmazonS3Client(awsCredentialsConfuguration.Credentials, awsCredentialsConfuguration.Region);
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IImageStorage, S3ImageStore>();

            return services;
        }
    }
}
