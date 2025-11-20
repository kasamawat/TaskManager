using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces;

namespace TaskManager.Application.Services
{
    public class AuthService : IAuthService
    {
        public Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
