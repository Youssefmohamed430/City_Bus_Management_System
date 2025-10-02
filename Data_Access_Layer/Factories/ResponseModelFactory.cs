using City_Bus_Management_System.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Factories
{
    public static class ResponseModelFactory<T> where T : class
    {
        public static ResponseModel<T> CreateResponse(string message, T result,bool IsSuccess = true)
        {
            return new ResponseModel<T>
            {
                IsSuccess = IsSuccess,
                Message = message,
                Result = result
            };
        }
    }
}
