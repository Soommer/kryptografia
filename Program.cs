
using kryptografia.Algorithms;
using kryptografia.Controllers;
using kryptografia.Services;

namespace kryptografia
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  policy =>
                                  {
                                      policy.AllowAnyOrigin()
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });


            builder.Services.AddControllers();
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


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("_myAllowSpecificOrigins"); 
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
