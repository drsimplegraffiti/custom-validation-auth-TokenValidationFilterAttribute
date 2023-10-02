using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthFilterProj.Data;
using AuthFilterProj.Interface;
using AuthFilterProj.Models;
using AuthFilterProj.Utils;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;

namespace AuthFilterProj.Service
{
    public class BookingRepository : IBookingRepository
    {
        private readonly Cloudinary _cloudinary;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        private readonly IUserRepository _userRepository;

        private readonly ILogger<ApartmentRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly IApartmentRepository _apartmentRepository;

        public BookingRepository(
                                   DataContext context,
                                   IConfiguration configuration,
                                   IUserRepository userRepository,
                                    IApartmentRepository apartmentRepository,
                                  ILogger<ApartmentRepository> logger,
                                  IHttpContextAccessor httpContextAccessor = null!,
                                  Cloudinary cloudinary = null!
                                 )
        {
            _context = context;
            _configuration = configuration;
            _userRepository = userRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _cloudinary = CloudinaryHelper.CreateCloudinaryInstance(_configuration);
            _apartmentRepository = apartmentRepository;
        }

        // public async Task<BookingCreateDtoResponse> CreateBooking(BookingCreateDto bookingCreateDto)
        // {
        //     try
        //     {
        //         var principal = _httpContextAccessor.HttpContext!.User;
        //         if (principal == null)
        //         {
        //             var r = new BookingCreateDtoResponse
        //             {
        //                 Message = "Error creating booking",
        //                 Success = false
        //             };

        //             return r;
        //         }
        //         var userIdClaim = principal.Identity!.Name;

        //         _logger.LogInformation("userIdClaim: " + userIdClaim);

        //         if (userIdClaim == null)
        //         {
        //             var r = new BookingCreateDtoResponse
        //             {
        //                 Message = "Error creating booking",
        //                 Success = false
        //             };

        //             return r;
        //         }


        //         var apartment = await _apartmentRepository.GetApartmentByIdAsync(bookingCreateDto.ApartmentId);

        //         if (apartment == null)
        //         {
        //             var r = new BookingCreateDtoResponse
        //             {
        //                 Message = "Error creating booking",
        //                 Success = false
        //             };

        //             return r;
        //         }

        //         var booking = new Booking
        //         {
                    
        //             UserId = int.Parse(userIdClaim),
        //             CheckIn = bookingCreateDto.CheckIn,
        //             CheckOut = bookingCreateDto.CheckOut,
        //             NoOfGuests = bookingCreateDto.NoOfGuests,
        //             Status = bookingCreateDto.Status,
        //             CreatedBy = userIdClaim,
        //             UpdatedBy = userIdClaim,
        //             ApartmentId = bookingCreateDto.ApartmentId,
        //         };

        //         await _context.Bookings.AddAsync(booking);
        //         await _context.SaveChangesAsync();

        //         var r2 = new BookingCreateDtoResponse
        //         {
        //             Message = "Booking created successfully",
        //             Success = true
        //         };

        //         return r2;
        //     }
        //     catch (Exception e)
        //     {
        //         Console.WriteLine(e);
        //         return new BookingCreateDtoResponse
        //         {
        //             Message = e.Message,
        //             Success = false
        //         };
        //     }
        // }

public async Task<BookingCreateDtoResponse> CreateBooking(BookingCreateDto bookingCreateDto)
{
    try
    {
        var principal = _httpContextAccessor.HttpContext!.User;
        if (principal == null)
        {
            var r = new BookingCreateDtoResponse
            {
                Message = "Error creating booking",
                Success = false
            };

            return r;
        }

        var userIdClaim = principal.Identity!.Name;

        if (string.IsNullOrEmpty(userIdClaim))
        {
            var r = new BookingCreateDtoResponse
            {
                Message = "Error creating booking",
                Success = false
            };

            return r;
        }

        var userId = int.Parse(userIdClaim);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            var r = new BookingCreateDtoResponse
            {
                Message = "User does not exist",
                Success = false
            };

            return r;
        }

        var apartment = await _apartmentRepository.GetApartmentByIdAsync(Convert.ToInt32(bookingCreateDto.ApartmentId));

        if (apartment == null)
        {
            var r = new BookingCreateDtoResponse
            {
                Message = "Error creating booking",
                Success = false
            };

            return r;
        }

        var booking = new Booking
        {
            UserId = int.Parse(userIdClaim),
            CheckIn = bookingCreateDto.CheckIn,
            CheckOut = bookingCreateDto.CheckOut,
            NoOfGuests = bookingCreateDto.NoOfGuests,
            Status = bookingCreateDto.Status,
            CreatedBy = userIdClaim,
            UpdatedBy = userIdClaim,
            ApartmentId = bookingCreateDto.ApartmentId,
        };

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        var r2 = new BookingCreateDtoResponse
        {
            Message = "Booking created successfully",
            Success = true
        };

        return r2;
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
        return new BookingCreateDtoResponse
        {
            Message = e.Message,
            Success = false
        };
    }
}


    }
}