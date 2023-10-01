using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthFilterProj.Data;
using AuthFilterProj.Dtos;
using AuthFilterProj.Dtos.Apartment;
using AuthFilterProj.Interface;
using AuthFilterProj.Models;
using AuthFilterProj.Utils;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace AuthFilterProj.Service
{
    public class ApartmentRepository : IApartmentRepository
    {
        private readonly Cloudinary _cloudinary;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        private readonly IUserRepository _userRepository;

        private readonly ILogger<ApartmentRepository> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;



        public ApartmentRepository(
                                    DataContext context,
                                    IConfiguration configuration,
                                    IUserRepository userRepository,
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
        }

        public async Task<ApartmentCreateDtoResponse> CreateApartment(ApartmentCreateDto apartmentCreateDto)
        {
            try
            {
                var principal = _httpContextAccessor.HttpContext!.User;
                if (principal == null)
                {
                    var r = new ApartmentCreateDtoResponse
                    {
                        Message = "Error creating apartment",
                        Success = false
                    };

                    return r;
                }

                var userIdClaim = principal.Identity!.Name;

                if (userIdClaim == null)
                {
                    var r = new ApartmentCreateDtoResponse
                    {
                        Message = "Error creating apartment",
                        Success = false
                    };

                    return r;
                }


                var apartment = new Apartment
                {
                    Name = apartmentCreateDto.Name,
                    Address = apartmentCreateDto.Address,
                    City = apartmentCreateDto.City,
                    State = apartmentCreateDto.State,
                    Country = apartmentCreateDto.Country,
                    ZipCode = apartmentCreateDto.ZipCode,
                    Phone = apartmentCreateDto.Phone,
                    Email = apartmentCreateDto.Email,
                    Website = apartmentCreateDto.Website,
                    Logo = apartmentCreateDto.Logo,
                    Description = apartmentCreateDto.Description,
                    Amenities = apartmentCreateDto.Amenities,
                    Rules = apartmentCreateDto.Rules,
                    Type = apartmentCreateDto.Type,
                    Status = apartmentCreateDto.Status,
                    CreatedBy = apartmentCreateDto.CreatedBy,
                    UpdatedBy = apartmentCreateDto.UpdatedBy,
                    UserId = int.Parse(userIdClaim)
                };

                // Add the apartment to the database
                _context.Apartments.Add(apartment);
                await _context.SaveChangesAsync();

                // You can return a response indicating success
                var response = new ApartmentCreateDtoResponse
                {

                    Message = "Apartment created successfully",
                    Success = true
                };

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating apartment: {ex}");
                // Handle the error and return an appropriate response
                var response = new ApartmentCreateDtoResponse
                {
                    Message = "Error creating apartment",
                    Success = false
                };

                return response;
            }
        }



        public Task<ApartmentCreateDtoResponse> UploadHouseLogoBanner(UpdateLogoDto updateLogoDto)
        {
            //get the authenticated user
            var principal = _httpContextAccessor.HttpContext!.User;
            if (principal == null)
            {
                throw new Exception("User not found");

            }

            var userIdClaim = principal.Identity!.Name;

            if (userIdClaim == null)
            {
                throw new Exception("User not found");
            }

            var user = _userRepository.GetUserById(int.Parse(userIdClaim));
            if (user == null)
            {
                throw new Exception("User not found");
            }

            //get the apartment
            var apartment = _context.Apartments.FirstOrDefault(x => x.UserId == user.Id);
            if (apartment == null)
            {
                throw new Exception("Apartment not found");
            }

            //update the apartment logo
            apartment.Logo = updateLogoDto.Logo;
            _context.Apartments.Update(apartment);
            _context.SaveChanges();

            //return response
            var response = new ApartmentCreateDtoResponse
            {
                Message = "Apartment logo updated successfully",
                Success = true
            };

            return Task.FromResult(response);
        }


public async Task<Response<List<string>>> UploadApartmentImages(List<IFormFile> files)
{
    // Get the authenticated user
    var principal = _httpContextAccessor.HttpContext?.User;
    if (principal == null)
    {
        return new Response<List<string>>
        {
            Message = "User not found",
            Success = false
        };
    }

    var userIdClaim = principal.Identity?.Name;

    if (string.IsNullOrEmpty(userIdClaim))
    {
        return new Response<List<string>>
        {
            Message = "User not found",
            Success = false
        };
    }

    var user = _userRepository.GetUserById(int.Parse(userIdClaim));
    if (user == null)
    {
        return new Response<List<string>>
        {
            Message = "User not found",
            Success = false
        };
    }

    // Get the apartment
    var apartment = _context.Apartments.FirstOrDefault(x => x.UserId == user.Id);
    if (apartment == null)
    {
        return new Response<List<string>>
        {
            Message = "Apartment not found",
            Success = false
        };
    }

    var uploadResults = new List<string>();

    foreach (var file in files)
    {
        if (file.Length > 0)
        {
            var uploadResult = new ImageUploadResult();

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill")
            };
            
            try
            {
                uploadResult = await _cloudinary.UploadAsync(uploadParams); // Use asynchronous Cloudinary upload
                _logger.LogInformation($"Image uploaded successfully: {uploadResult.Url}");

                apartment.UpdatedBy = user.Name;
                apartment.UpdatedAt = DateTime.Now;
                apartment.ApartmentImages.Add(uploadResult.Url.ToString());
                _context.Apartments.Update(apartment);
                await _context.SaveChangesAsync(); // Use asynchronous SaveChangesAsync
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during upload
                _logger.LogError($"Image upload failed: {ex.Message}");
                return new Response<List<string>>
                {
                    Success = false,
                    Message = "Image upload failed",
                    Data = uploadResults
                };
            }
        }
    }

    return new Response<List<string>>
    {
        Success = true,
        Message = "Images uploaded successfully",
        Data = apartment.ApartmentImages
    };
}

        public async Task<Apartment> GetApartmentByIdAsync(int id)
        {
            var apartment = await _context.Apartments.FindAsync(id);
            if (apartment == null)
            {
                throw new Exception("Apartment not found");
            }

            return apartment;
        }
    }
}