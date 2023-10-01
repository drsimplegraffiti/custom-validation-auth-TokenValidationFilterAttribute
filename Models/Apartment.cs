using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AuthFilterProj.Models
{
    public class Apartment: BaseEntity
    {
       public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Website { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Logo { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Amenities { get; set; } = new List<string>();
        public List<string> Rules { get; set; } = new List<string>();

        //images
        public List<string> ApartmentImages { get; set; } = new List<string>();


        [ForeignKey("User")]
        public int UserId { get; set; }

        [JsonIgnore]
        public User User { get; set; } = default!;

        
       
    }

}