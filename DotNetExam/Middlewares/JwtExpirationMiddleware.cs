using DotNetExam.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace DotNetExam.Middlewares
{
    public class JwtExpirationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtExpirationMiddleware> _logger;
        private readonly ITokenService _tokenService;
        public JwtExpirationMiddleware(RequestDelegate next, ILogger<JwtExpirationMiddleware> logger, ITokenService tokenService)
        {
            _next = next;
            _logger = logger;
            _tokenService = tokenService;
        }
        public async Task Invoke(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length);
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(_tokenService.GetSecurityKey()),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                try
                {
                    SecurityToken securityToken;
                    var principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);
                    if (securityToken.ValidTo < DateTime.UtcNow)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return;
                    }

                    context.User = principal;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Invalid token");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return;
                }
            }
            await _next(context);
        }
    }
}
