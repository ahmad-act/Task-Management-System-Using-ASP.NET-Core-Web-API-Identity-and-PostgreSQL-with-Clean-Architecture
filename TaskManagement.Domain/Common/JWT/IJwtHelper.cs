using Microsoft.AspNetCore.Http;

namespace TaskManagement.Domain.Common.JWT
{
    public interface IJwtHelper
    {
        string GenerateJwt(string id, string role);
        CookieOptions GetCookieOption();
        JwtData GetJwt(string cookie);
    }
}