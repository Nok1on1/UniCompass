using Microsoft.OpenApi.Models;
using Supabase;

namespace UniCompass.config
{
    public static class CustomBuilderConfig
    {
        public static WebApplicationBuilder ApplyConfig(WebApplicationBuilder builder)
        {
            //
            var pgUrl = builder.Configuration["SUPABASE_URL"] ?? "";
            var userPgKey = builder.Configuration["SUPABASE_KEY"] ?? "";
            var adminPgKey = builder.Configuration["SUPABASE_ADMIN_KEY"] ?? "";

            if (
                string.IsNullOrEmpty(pgUrl)
                || string.IsNullOrEmpty(userPgKey)
                || string.IsNullOrEmpty(adminPgKey)
            )
            {
                throw new InvalidOperationException(
                    "SUPABASE_URL and SUPABASE_KEY must be configured in appsettings.json"
                );
            }

            var pgOptions = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
            };

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "UniCompass API",
                        Version = "v1",
                        Description = "API for UniCompass application",
                    }
                );
                c.MapType<IFormFile>(() =>
                    new OpenApiSchema { Type = "string", Format = "binary" }
                );
            });

            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddKeyedSingleton(
                "user",
                (_, _) => new Client(pgUrl, userPgKey, pgOptions)
            );
            builder.Services.AddKeyedSingleton(
                "admin",
                (_, _) => new Client(pgUrl, adminPgKey, pgOptions)
            );

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAllOrigins",
                    builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                );
            });

            return builder;
        }
    }
}
