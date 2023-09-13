using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace BranchCrudAPI_JWT.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secret;
        private readonly List<string> _excludePaths;

        public TokenValidationMiddleware(RequestDelegate next, string secret)
        {
            _next = next;
            _secret = secret;

            _excludePaths = new List<string>
        {
            "/api/Users/login",
             "/api/Users/register"
        };
        }

        public async Task InvokeAsync(HttpContext context)
        {

            if (ShouldExcludePath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            // Check if the Authorization header is present
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";

                var responseMessage = new
                {
                    error = "Token is missing."
                };

                var jsonResponse = JsonConvert.SerializeObject(responseMessage);
                await context.Response.WriteAsync(jsonResponse);
                return;
            }

            //for invalid token
            var token = context.Request.Headers["Authorization"].ToString().Split(" ")[1];

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var secret = _secret;
                var key = Encoding.ASCII.GetBytes(secret);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var claims = jwtToken.Claims;

                //await _next(context); // Continue processing the request //don't repeat _next()
            }
            catch (Exception)
            {
                context.Response.StatusCode = 401; // Unauthorized
                context.Response.ContentType = "application/json";

                var responseMessage = new
                {
                    error = "Invalid Token"
                };

                var jsonResponse = JsonConvert.SerializeObject(responseMessage);
                await context.Response.WriteAsync(jsonResponse);
                return;
            }


            // Continue processing the request
            await _next(context);
        }


        private bool ShouldExcludePath(PathString path)
        {
            return _excludePaths.Contains(path);
        }

    }
}
