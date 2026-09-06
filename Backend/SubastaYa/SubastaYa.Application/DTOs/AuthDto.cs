using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.DTOs
{
    public record AuthDto(string Token, DateTime ExpiresAt, Guid UserId, string Name, string Email);
}

