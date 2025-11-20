using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Auth;

namespace TaskManager.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request);
    }
}
