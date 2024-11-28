using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.Interfaces.Services;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class LessonStatusController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/lessonStatuses";

        private readonly ILessonStatusService _lessonStatusService;

        public LessonStatusController(ILessonStatusService lessonStatusService)
        {
            _lessonStatusService = lessonStatusService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonStatusReadDTO>>> GetLessonStatuses(CancellationToken cancellationToken)
        {
            var lessonStatuses = await _lessonStatusService.GetLessonStatuses(cancellationToken);

            return Ok(lessonStatuses);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonStatusReadDTO>> GetLessonStatusById(int id, CancellationToken cancellationToken)
        {
            var lessonStatus = await _lessonStatusService.GetLessonStatusById(id, cancellationToken);

            return Ok(lessonStatus);
        }
    }
}
