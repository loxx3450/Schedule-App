using Schedule_App.Core.DTOs.Auth;
using Schedule_App.Core.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<AuthResponse> Login(TeacherLoginDTO loginDTO, CancellationToken cancellationToken);
    }
}
