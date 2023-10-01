using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthFilterProj.Dtos;
using AuthFilterProj.Dtos.Apartment;
using AuthFilterProj.Models;

namespace AuthFilterProj.Interface
{
    public interface IApartmentRepository
    {
        Task<ApartmentCreateDtoResponse> CreateApartment(ApartmentCreateDto apartmentCreateDto);
        
        //upload house logo banner
        Task<ApartmentCreateDtoResponse> UploadHouseLogoBanner(UpdateLogoDto updateLogoDto);

        // upload apartment images (multiple)
            Task<Response<List<string>>> UploadApartmentImages(List<IFormFile> files);

        //GetApartmentByIdAsync
        Task<Apartment> GetApartmentByIdAsync(int id);

     
}

    public class UpdateImagesDto
    {
        public List<string> Images { get; set; } = new List<string>();
    }

    public class UpdateLogoDto
    {
        public string Logo { get; set; } = string.Empty;
    }
}