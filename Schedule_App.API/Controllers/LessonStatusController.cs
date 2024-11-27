using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.Interfaces;

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
            var result = await _lessonStatusService.GetLessonStatuses(cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonStatusReadDTO>> GetLessonStatusById(int id, CancellationToken cancellationToken)
        {
            var result = await _lessonStatusService.GetLessonStatusById(id, cancellationToken);

            return Ok(result);
        }
    }
}
