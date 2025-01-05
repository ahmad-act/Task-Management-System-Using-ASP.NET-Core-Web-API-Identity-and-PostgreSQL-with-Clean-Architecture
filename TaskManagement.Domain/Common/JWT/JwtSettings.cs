using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TaskManagement.Domain.Common.JWT
{
    /// <summary>
    /// Represents the settings used for configuring JWT (JSON Web Token) authentication.
    /// This class contains properties that define how the JWT tokens are generated, validated, and configured.
    /// </summary>
    public class JwtSettings : IJwtHelper
    {
        /// <summary>
        /// Gets or sets the name of the JWT token.
        /// This property defines the name used to reference the token in HTTP requests.
        /// </summary>
        public string JwtTokenName { get; set; }

        /// <summary>
        /// Gets or sets the secret key used for signing the JWT.
        /// This key is essential for ensuring the integrity and authenticity of the token.
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// Gets or sets the issuer of the JWT.
        /// This property specifies the entity that issues the token, which can be used for validation.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the issuer of the token.
        /// If true, the token's issuer will be validated against the configured issuer.
        /// </summary>
        public bool ValidateIssuer { get; set; }

        /// <summary>
        /// Gets or sets the audience for the JWT.
        /// This property defines the intended recipient(s) of the token.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the audience of the token.
        /// If true, the token's audience will be validated against the configured audience.
        /// </summary>
        public bool ValidateAudience { get; set; }

        /// <summary>
        /// Gets or sets the expiration time of the token in minutes.
        /// This property determines how long the token remains valid before it expires.
        /// </summary>
        public int TokenExpirationInMinutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the lifetime of the token.
        /// If true, the token's expiration will be checked to ensure it is still valid.
        /// </summary>
        public bool ValidateLifetime { get; set; }

        /// <summary>
        /// Generates a JWT token for the specified user ID and role.
        /// </summary>
        /// <param name="_httpContextAccessor">The HTTP context accessor used to access the current HTTP context.</param>
        /// <param name="id">The unique identifier of the user for whom the token is generated.</param>
        /// <param name="role">The role of the user, used for authorization purposes.</param>
        /// <param name="jwtSettings">The settings for configuring the JWT token generation.</param>
        /// <returns>A string representation of the generated JWT token.</returns>
        public string GenerateJwt(string id, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, id),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(TokenExpirationInMinutes),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Decodes and validates the given JWT token string, returning the associated user ID and role.
        /// </summary>
        /// <param name="cookie">The JWT token string to be validated and decoded.</param>
        /// <param name="jwtSettings">The settings for validating the JWT token.</param>
        /// <returns>A <see cref="JwtData"/> object containing the user ID and role from the token.</returns>
        public JwtData GetJwt(string cookie)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            // Create validation parameters with the secret key
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateLifetime = true
            };

            // Validate and decode the token
            var principal = tokenHandler.ValidateToken(cookie, validationParameters, out SecurityToken validatedToken);
            var id = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            return new JwtData() { id = id, role = role };
        }

        /// <summary>
        /// Configures the options for setting a cookie that contains the JWT token.
        /// </summary>
        /// <param name="jwtSettings">The settings that define the cookie options.</param>
        /// <returns>A <see cref="CookieOptions"/> object configured for JWT token cookies.</returns>
        public CookieOptions GetCookieOption()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(TokenExpirationInMinutes),
                SameSite = SameSiteMode.Strict
            };

            return cookieOptions;
        }
    }
}
