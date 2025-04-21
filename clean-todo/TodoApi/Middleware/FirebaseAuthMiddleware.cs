using System.Security.Claims;
using FirebaseAdmin.Auth;

namespace TodoApi
{
    public class FirebaseAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public FirebaseAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                try
                {
                    var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                    var uid = decodedToken.Uid;

                    var claims = new List<Claim>
                    {
                        new Claim("user_id", uid)
                    };

                    var identity = new ClaimsIdentity(claims, "firebase");
                    context.User = new ClaimsPrincipal(identity);
                }
                catch
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Yetkilendirme başarısız.");
                    return;
                }
            }

            await _next(context);
        }
    }

    public static class FirebaseAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseFirebaseAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<FirebaseAuthMiddleware>();
        }
    }
}