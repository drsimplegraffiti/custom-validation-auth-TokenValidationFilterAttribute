using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Dtos
{
    // public class LoginDto
    // {
    //     public string Email { get; set; } = string.Empty;
    //     public string Password { get; set; } = string.Empty;
    // }

    public record LoginDto(
        string Email,
        string Password);

    // add annotations to the record using {}
    public record LoginDto2
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

}