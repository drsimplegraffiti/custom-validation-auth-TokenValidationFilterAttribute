using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthFilterProj.Custom;
using AuthFilterProj.Dtos.Apartment;
using AuthFilterProj.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AuthFilterProj.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(TokenValidationFilterAttribute))] // Apply the token validation filter
    public class ApartmentController : ControllerBase
    {
        private readonly IApartmentRepository _apartmentRepository;

        public ApartmentController(IApartmentRepository apartmentRepository)
        {
            _apartmentRepository = apartmentRepository;
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(ApartmentCreateDtoResponse), 200)]
        [ProducesResponseType(typeof(ApartmentCreateDtoResponse), 400)]
        [ProducesResponseType(typeof(ApartmentCreateDtoResponse), 500)]
        public async Task<IActionResult> CreateApartment([FromBody] ApartmentCreateDto apartmentCreateDto)
        {
            var result = await _apartmentRepository.CreateApartment(apartmentCreateDto);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        //upload apartment logo
        [HttpPost("upload-logo")]
        public async Task<IActionResult> UploadLogo([FromForm] UpdateLogoDto uploadLogoDto)
        {
            var response = await _apartmentRepository.UploadHouseLogoBanner(uploadLogoDto);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("upload-images/multiple")]
        public async Task<ActionResult<List<string>>> UploadApartmentImages(List<IFormFile> files)
        {
            var response = await _apartmentRepository.UploadApartmentImages(files);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }


        }
    }
}