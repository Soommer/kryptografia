
using kryptografia.Algorithms;
using kryptografia.Controllers;
using kryptografia.Services;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace kryptografia
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod()
                                            .WithExposedHeaders("Czas")
                                            .WithExposedHeaders("Ram");
                                  });
            });


            builder.Services.AddControllers();
            builder.Host.UseSerilog(); 
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
            });

            builder.Services.AddScoped<IEncryptionService, EncryptionService>();
            builder.Services.AddScoped<ISteganography, SteganographyEncoder>();
            builder.Services.AddScoped<ISteganographyDecoder, SteganographyDecoder>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<EncryptionService>();
            builder.Services.AddSingleton<EncryptionFactory>();
            builder.Services.AddTransient<VigenereCipher>();
            builder.Services.AddTransient<CaesarCipher>();
            builder.Services.AddTransient<Sha256Hash>();
            builder.Services.AddTransient<AesEncryption>();
            builder.Services.AddTransient<RsaEncryption>();
            builder.Services.AddTransient<SteganographyEncoder>();
            builder.Services.AddTransient<SteganographyDecoder>();




            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                    Log.Error(exception, "Unhandled exception");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("Something went wrong");
                });
            });


            app.UseHttpsRedirection();
            app.UseCors("_myAllowSpecificOrigins"); 
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
