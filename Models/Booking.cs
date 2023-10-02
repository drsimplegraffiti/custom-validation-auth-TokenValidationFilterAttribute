using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthFilterProj.Models
{
    public class Booking: BaseEntity
    {
        
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NoOfGuests { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;

        [ForeignKey("User")]
        [Required] // Add this annotation to make the foreign key required
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; } = default!;

        [ForeignKey("Apartment")]
        public int ApartmentId { get; set; }

        [JsonIgnore]
        public Apartment Apartment { get; set; } = default!;


    }
}