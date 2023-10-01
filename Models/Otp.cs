using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Models
{
    public class Otp: BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public DateTime ExpirationDateTime { get; set; } = DateTime.UtcNow.AddMinutes(5);
    }
}