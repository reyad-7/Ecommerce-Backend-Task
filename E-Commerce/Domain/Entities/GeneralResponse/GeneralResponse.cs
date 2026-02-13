using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.GeneralResponse
{
    public class GeneralResponse
    {
        public GeneralResponse() { }

        public class GeneralResponseDto<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }

            public GeneralResponseDto(bool success, string? message = null, T? data = default)
            {
                Success = success;
                Message = message;
                Data = data;
            }
            public static GeneralResponseDto<T> SuccessResponse(T? data = default, string? message = null)
            {
                return new GeneralResponseDto<T>(true, message, data);
            }
            public static GeneralResponseDto<T> FailureResponse(string message)
            {
                return new GeneralResponseDto<T>(false, message);
            }
        }
    }
}
