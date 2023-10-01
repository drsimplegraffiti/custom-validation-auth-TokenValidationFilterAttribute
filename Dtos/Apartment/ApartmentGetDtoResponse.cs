using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthFilterProj.Dtos.Apartment;

namespace AuthFilterProj.Dtos
{
    public class ApartmentGetDtoResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = false;
        public ApartmentGetDto ApartmentGetDto { get; set; } = new ApartmentGetDto();
    }
}