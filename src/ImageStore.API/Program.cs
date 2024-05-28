using Amazon.S3;
using ImageStore.API.Services;
using ImageStore.Infrastructure;
using ImageStore.Infrastructure.Configuration;
namespace ImageStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationDependencies();

            var awsCredentialsConfuguration = GetAwsCredentialsConfuguration(builder.Configuration);
            builder.Services.AddSingleton<AwsCredentialsConfuguration>(x => awsCredentialsConfuguration);

            builder.Services.AddInfrastructureServices(builder.Configuration, awsCredentialsConfuguration);

            builder.Services.AddHostedService<SQSProcessedImageProcessor>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        public static AwsCredentialsConfuguration GetAwsCredentialsConfuguration(IConfiguration configuration)
        {
            var accessKey = configuration["AWS:AccessKey"];
            var secretKey = configuration["AWS:SecretKey"];
            var region = configuration["AWS:Region"];
            return new AwsCredentialsConfuguration(accessKey, secretKey, region);
        }
    }
}
