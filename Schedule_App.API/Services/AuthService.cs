using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Auth;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;
using System.Security.Claims;
using System.Text;

namespace Schedule_App.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository _repository;
        private readonly TokenProvider _tokenProvider;

        public AuthService(IRepository repository, TokenProvider tokenProvider)
        {
            _repository = repository;
            _tokenProvider = tokenProvider;
        }

        public async Task<AuthResponse> Login(TeacherLoginDTO loginDTO, CancellationToken cancellationToken)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Username == loginDTO.Username, cancellationToken);

            // If teacher with this Username exists
            if (teacher is not null)
            {
                // If password is correct
                if (PasswordHasher.Verify(loginDTO.Password, teacher.Password))
                {
                    return new AuthResponse()
                    {
                        Token = _tokenProvider.GenerateToken(teacher)
                    };
                }
            }

            throw new ArgumentException("Username or password is incorrect");
        }
    }
}
