using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Supabase.Gotrue;

namespace UniCompass.Middleware;

public class SupabaseSessionMiddleware
{
    private readonly RequestDelegate _next;

    public SupabaseSessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/api/Auth"))
        {
            await _next(context);
            return;
        }

        var supabase = context.RequestServices.GetKeyedService<Supabase.Client>("user");

        if (supabase == null)
        {
            await _next(context);
            return;
        }

        // Fix cookie names to match your login controller
        var accessToken = context.Request.Cookies["x-access-token"];
        var refreshToken = context.Request.Cookies["x-refresh-token"];

        if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
        {
            await _next(context);
            return;
        }

        await supabase.Auth.SetSession(accessToken, refreshToken);

        await _next(context);
    }
}
