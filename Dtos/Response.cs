using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthFilterProj.Dtos
{
    public class Response<T>
    {
        public T Data { get; set; } = default!;
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }

    // Error response
    public class ErrorResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = false;
        public string? Error { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;


    }
}