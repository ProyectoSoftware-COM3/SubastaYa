using Microsoft.AspNetCore.Http;
using SubastaYa.Application.Common.Interfaces;

namespace SubastaYa.Infrastructure.Security
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
            => _httpContextAccessor = httpContextAccessor;

        public Guid UserId
        {
            get
            {
                var sub = _httpContextAccessor.HttpContext?.User
                    .FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
                return sub is not null ? Guid.Parse(sub) : Guid.Empty;
            }
        }

        public bool IsAuthenticated
            => _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    }
}