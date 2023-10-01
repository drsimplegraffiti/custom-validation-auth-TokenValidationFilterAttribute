using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Dtos
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
}