
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthFilterProj.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool? IsVerified { get; set; } = false;
        public string Role { get; set; } = "User";
        [JsonIgnore]
        public List<Apartment> Apartments { get; set; } = new List<Apartment>();
        public User()
        {
            Role = "User";
        }

    }

    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

   
}