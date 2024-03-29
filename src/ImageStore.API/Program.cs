using ImageStore.API.Configuration;
using ImageStore.API.Services;
using ImageStore.Infrastructure;
using ImageStore.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ImageStore.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddApplicationDependencies(builder.Configuration);

            var awsCredentialsConfuguration = GetAwsCredentialsConfuguration(builder.Configuration);
            builder.Services.AddSingleton<AwsCredentialsConfuguration>(x => awsCredentialsConfuguration);

            builder.Services.AddInfrastructureServices(builder.Configuration, awsCredentialsConfuguration);

            builder.Services.Configure<ProcessedImagesQueueConfig>(builder.Configuration.GetRequiredSection("AWS:ProcessedImagesQueue"));
            builder.Services.AddHostedService<SQSProcessedImageProcessor>();

            var jwtIssuer = builder.Configuration.GetRequiredSection("Jwt:Issuer").Get<string>();
            var jwtKey = builder.Configuration.GetRequiredSection("Jwt:Key").Get<string>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwtIssuer,
                     ValidAudience = jwtIssuer,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                 };
             });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();
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
