using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}