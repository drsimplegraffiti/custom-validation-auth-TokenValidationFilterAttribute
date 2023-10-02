using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthFilterProj.Custom;
using AuthFilterProj.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthFilterProj.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(TokenValidationFilterAttribute))] // Apply the token validation filter
    public class BookingController: ControllerBase
    {
        
        private readonly IBookingRepository _bookingRepository;

        public BookingController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(BookingCreateDtoResponse), 200)]
        [ProducesResponseType(typeof(BookingCreateDtoResponse), 400)]
        [ProducesResponseType(typeof(BookingCreateDtoResponse), 500)]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingCreateDto)
        {
            var result = await _bookingRepository.CreateBooking(bookingCreateDto);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}