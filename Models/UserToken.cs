
using System.Text.Json.Serialization;

namespace AuthFilterProj.Models
{
    public class UserToken: BaseEntity
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;

        // expires in 1 hour
        public DateTime ExpirationDateTime { get; set; } = DateTime.UtcNow.AddHours(1);
        // userId
        public int UserId { get; set; }

        // navigation property
        [JsonIgnore]
        public User User { get; set; } = null!;

    }
}