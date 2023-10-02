
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
        public string PublicId { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;

        public string UserAgent { get; set; } = string.Empty;

        public int NoOfLoginTries { get; set; } = 0;
        public bool IsLocked { get; set; } = false;
        public string Role { get; set; } = "User";
        [JsonIgnore]
        public List<Apartment> Apartments { get; set; } = new List<Apartment>();

        [JsonIgnore]
        public List<Booking> Bookings { get; set; } = new List<Booking>();


        //google auth properties
        public string GoogleId { get; set; } = string.Empty;
        public string GoogleEmail { get; set; } = string.Empty;
        public string GoogleName { get; set; } = string.Empty;
        public string GooglePictureUrl { get; set; } = string.Empty;
        public string GoogleLocale { get; set; } = string.Empty;




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