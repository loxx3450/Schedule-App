using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class LessonStatusController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/lessonStatuses";

        private readonly ILessonStatusService _lessonStatusService;
        private readonly IMapper _mapper;

        public LessonStatusController(ILessonStatusService lessonStatusService, IMapper mapper)
        {
            _lessonStatusService = lessonStatusService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonStatusReadDTO>>> GetLessonStatuses(CancellationToken cancellationToken)
        {
            LessonStatus[] lessonStatuses = await _lessonStatusService.GetLessonStatuses(cancellationToken);

            var result = _mapper.Map<IEnumerable<LessonStatusReadDTO>>(lessonStatuses);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonStatusReadDTO>> GetLessonStatusById(int id, CancellationToken cancellationToken)
        {
            LessonStatus lessonStatus = await _lessonStatusService.GetLessonStatusById(id, cancellationToken);

            var result = _mapper.Map<LessonStatusReadDTO>(lessonStatus);

            return Ok(result);
        }
    }
}
