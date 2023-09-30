using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Dtos
{
    public class CreateUserDto
    {
    
    [Required (ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required (ErrorMessage = "Email is required")]
    [EmailAddress (ErrorMessage = "Email is not valid")]
    [DataType(DataType.EmailAddress)]
    [StringLength(50, ErrorMessage = "Email cannot be longer than 50 characters")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email is not valid")]
    public string Email { get; set; } = string.Empty;

    [Required (ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(50, ErrorMessage = "Password cannot be longer than 50 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,50}$", ErrorMessage = "Password must contain at least 8 characters, one uppercase, one lowercase, one number and one special character")]
    public string Password { get; set; } = string.Empty;
    }
}