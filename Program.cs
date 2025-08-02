using Microsoft.AspNetCore.HttpOverrides;
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
        app.UseForwardedHeaders(
            new ForwardedHeadersOptions
            {
                ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            }
        ); 

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<SupabaseSessionMiddleware>();

        app.UseWhen(
            context => context.Request.Path.StartsWithSegments("/api/Admin"),
            adminApp => adminApp.UseMiddleware<AdminCheckMiddleware>()
        );

        app.MapControllers();

        app.Run();
    }
}
