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

        public LessonController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LessonReadSummaryDTO>>> GetTeachers(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool withDetailed = false,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<LessonReadSummaryDTO> lessons;

            if (withDetailed)
            {
                lessons = await _lessonService.GetLessonsDetailed(offset, limit, cancellationToken);
            }
            else
            {
                lessons = await _lessonService.GetLessonsSummaries(offset, limit, cancellationToken);
            }

            return Ok(lessons);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<LessonReadSummaryDTO>>> GetTeachersByFilter(
            [FromQuery] int? classroomId,
            [FromQuery] int? subjectId,
            [FromQuery] int? groupId,
            [FromQuery] int? teacherId,
            [FromQuery] DateOnly? date,
            [FromQuery] int? statusId,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool withDetailed = false,
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

            IEnumerable<LessonReadSummaryDTO> lessons;

            if (withDetailed)
            {
                lessons = await _lessonService.GetLessonsDetailedByFilter(lessonFilter, offset, limit, cancellationToken);
            }
            else
            {
                lessons = await _lessonService.GetLessonsSummariesByFilter(lessonFilter, offset, limit, cancellationToken);
            }

            return Ok(lessons);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonReadSummaryDTO>> GetLessonById(
            [FromRoute] int id,
            [FromQuery] bool withDetailed = false,
            CancellationToken cancellationToken = default)
        {
            LessonReadSummaryDTO lesson;

            if (withDetailed)
            {
                lesson = await _lessonService.GetLessonDetailedById(id, cancellationToken);
            }   
            else
            {
                lesson = await _lessonService.GetLessonSummaryById(id, cancellationToken);
            }

            return Ok(lesson);
        }

        [HttpPost]
        public async Task<ActionResult<LessonReadSummaryDTO>> AddLesson([FromBody] LessonCreateDTO lessonCreateDTO, CancellationToken cancellationToken)
        {
            var lesson = await _lessonService.AddLesson(lessonCreateDTO, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{lesson.Id}", lesson);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<LessonReadSummaryDTO>> UpdateLesson([FromRoute] int id, [FromBody] LessonUpdateDTO lessonUpdateDTO, CancellationToken cancellationToken)
        {
            var lesson = await _lessonService.UpdateLesson(id, lessonUpdateDTO, cancellationToken);

            return Ok(lesson);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteLesson(int id, CancellationToken cancellationToken)
        {
            await _lessonService.DeleteLesson(id, cancellationToken);

            return NoContent();
        }
    }
}
