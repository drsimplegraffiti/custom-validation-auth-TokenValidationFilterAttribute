using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Interface
{
    public interface IBookingRepository
    {
        Task<BookingCreateDtoResponse> CreateBooking(BookingCreateDto bookingCreateDto);
        
     
    }

    public class BookingCreateDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NoOfGuests { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string UpdatedBy { get; set; } = string.Empty;

     public int ApartmentId { get; set; }
    }

    public class BookingCreateDtoResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = false;

        
    }
}