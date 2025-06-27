using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Uttt.Micro.Cupon.Extensions  
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            var settingsSection = builder.Configuration.GetSection("ApiSettings");
            var secret = settingsSection.GetValue<string>("JwtOptions:Secret");
            var issuer = settingsSection.GetValue<string>("JwtOptions:Issuer");
            var audience = settingsSection.GetValue<string>("JwtOptions:Audience");

            var key = Encoding.UTF8.GetBytes(secret); // Cambiado a UTF8

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                // Para debugging detallado
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"🔴 Authentication failed: {context.Exception.Message}");
                        if (context.Exception.InnerException != null)
                        {
                            Console.WriteLine($"🔴 Inner Exception: {context.Exception.InnerException.Message}");
                        }
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("✅ Token validated successfully");
                        var claims = context.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                        Console.WriteLine($"Claims: {string.Join(", ", claims)}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"🔴 JWT Challenge: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                        Console.WriteLine($"🔍 Authorization Header: '{authHeader}'");

                        var token = context.Token;
                        Console.WriteLine($"🔍 Extracted Token: '{(token?.Length > 50 ? token.Substring(0, 50) + "..." : token ?? "NULL")}'");
                        Console.WriteLine($"🔍 Token Length: {token?.Length ?? 0}");

                        if (string.IsNullOrEmpty(token))
                        {
                            Console.WriteLine("🔴 TOKEN IS NULL OR EMPTY!");
                        }
                        else if (!token.Contains('.'))
                        {
                            Console.WriteLine("🔴 TOKEN DOESN'T CONTAIN DOTS!");
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return builder;
        }
    }
}