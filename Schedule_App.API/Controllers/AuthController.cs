using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Auth;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Interfaces;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    public class AuthController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/auth";

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(TeacherLoginDTO loginDTO, CancellationToken cancellationToken)
        {
            var response = await _authService.Login(loginDTO, cancellationToken);

            return Ok(response);
        }
    }
}
