using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs.Auth;
using TaskManager.Application.Interfaces;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            // check duplicate email
            var exitsing = await _userRepository.GetByEmailAsync(request.Email);
            if(exitsing != null)
            {
                return ServiceResult<AuthResponse>.Fail("Email already exists.");
            }

            // Hash Password
            var passwordHash = _passwordHasher.Hash(request.Password);

            // Create User Entity
            var user = new User(
                request.FirstName,
                request.LastName,
                request.Email,
                passwordHash
            );

            await _userRepository.AddAsync(user);

            // Generate Token
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);

            var response = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
            };

            return ServiceResult<AuthResponse>.Success(response);
        }
        public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request)
        {
            // Find User From Email
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if(user == null)
            {
                return ServiceResult<AuthResponse>.Fail("Invalid email or password.");
            }

            // Check Password
            var isValidPassword = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);
            if (!isValidPassword)
            {
                return ServiceResult<AuthResponse>.Fail("Invalid email or password");
            }

            // Generate Token
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email);

            var response = new AuthResponse
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
            };

            return ServiceResult<AuthResponse>.Success(response);
        }
    }
}
