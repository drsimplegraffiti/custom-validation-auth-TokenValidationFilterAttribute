using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Utils
{
    public class AppSettings
    {
        public string EmailFrom { get; set; } = string.Empty;
        public string SmtpHost { get; set; } = string.Empty;
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; } = string.Empty;
        public string SmtpPass { get; set; } = string.Empty;
    }
}