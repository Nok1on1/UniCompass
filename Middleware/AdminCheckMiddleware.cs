using System;

namespace UniCompass.Middleware;

public class AdminCheckMiddleware
{
    private readonly RequestDelegate _next;

    public AdminCheckMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var _supabase = context.RequestServices.GetKeyedService<Supabase.Client>("user");

        Guid.TryParse(_supabase.Auth.CurrentUser?.Id, out Guid supabaseUserId);

        if (supabaseUserId == Guid.Empty)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized access.");
            return;
        }

        var user = await _supabase
            .From<Models.Users>()
            .Where(x => x.UserId == supabaseUserId && x.UserType == UserType.ADMIN)
            .Single();

        if (user == null || user.UserType != UserType.ADMIN)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Access denied. Admins only.");
            return;
        }

        await _next(context);
    }
}
