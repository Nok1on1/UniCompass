using UniCompass.config;
using UniCompass.Middleware;

namespace UniCompass;

public static class Program
{
    static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder = CustomBuilderConfig.ApplyConfig(builder);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseMiddleware<SupabaseSessionMiddleware>();

        app.MapControllers();

        app.Run();
    }
}
