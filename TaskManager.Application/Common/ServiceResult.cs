using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Application.Common
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public T? Data { get; }

        private ServiceResult(bool isSuccess, T? data, string? error)
        {
            IsSuccess = isSuccess;
            Error = error;
            Data = data;
        }
        public static ServiceResult<T> Success(T data) => new ServiceResult<T>(true, data, null);
        public static ServiceResult<T> Fail(string error) => new ServiceResult<T>(false, default, error);
    }
}
