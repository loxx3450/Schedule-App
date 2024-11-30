using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class LessonController : ControllerBase
    {
        private const string BASE_ENDPOINT = "lessons";

        private readonly ILessonService _lessonService;
        private readonly IMapper _mapper;

        public LessonController(ILessonService lessonService, IMapper mapper)
        {
            _lessonService = lessonService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonReadSummaryDTO>>> GetLessons(
            [FromQuery] int? classroomId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] DateOnly? date = null,
            [FromQuery] int? statusId = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            var lessonFilter = new LessonFilter()
            {
                ClassroomId = classroomId,
                SubjectId = subjectId,
                GroupId = groupId,
                TeacherId = teacherId,
                StartDate = date,
                StatusId = statusId,
            };

            Lesson[] lessons = await _lessonService.GetLessons(lessonFilter, offset, limit, cancellationToken);

            IEnumerable<LessonReadSummaryDTO> result;

            if (includeAuditInfo)
                result = _mapper.Map<IEnumerable<LessonReadFullDTO>>(lessons);
            else
                result = _mapper.Map<IEnumerable<LessonReadSummaryDTO>>(lessons);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonReadSummaryDTO>> GetLessonById(
            [FromRoute] int id,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            Lesson lesson = await _lessonService.GetLessonById(id, cancellationToken);

            LessonReadSummaryDTO result;

            if (includeAuditInfo)
                result = _mapper.Map<LessonReadFullDTO>(lesson);
            else
                result = _mapper.Map<LessonReadSummaryDTO>(lesson);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<LessonReadSummaryDTO>> AddLesson([FromBody] LessonCreateDTO lessonCreateDTO, CancellationToken cancellationToken)
        {
            var lesson = _mapper.Map<Lesson>(lessonCreateDTO);

            Lesson createdLesson = await _lessonService.AddLesson(lesson, cancellationToken);

            var result = _mapper.Map<LessonReadSummaryDTO>(createdLesson);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<LessonReadSummaryDTO>> UpdateLesson(
            [FromRoute] int id, 
            [FromBody] LessonUpdateDTO lessonUpdateDTO, 
            CancellationToken cancellationToken)
        {
            Lesson lesson = await _lessonService.UpdateLesson(id, lessonUpdateDTO, cancellationToken);

            var result = _mapper.Map<LessonReadSummaryDTO>(lesson);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteLesson(int id, CancellationToken cancellationToken)
        {
            await _lessonService.DeleteLesson(id, cancellationToken);

            return NoContent();
        }
    }
}
