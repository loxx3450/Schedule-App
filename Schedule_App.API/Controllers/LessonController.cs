using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
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
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<LessonReadSummaryDTO> lessons;

            if (withDetails)
            {
                lessons = await _lessonService.GetLessonsDetails(skip, take, cancellationToken);
            }
            else
            {
                lessons = await _lessonService.GetLessonsSummaries(skip, take, cancellationToken);
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
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            var lessonFilter = new LessonFilter()
            {
                ClassroomId = classroomId,
                SubjectId = subjectId,
                GroupId = groupId,
                TeacherId = teacherId,
                Date = date,
                StatusId = statusId,
            };

            IEnumerable<LessonReadSummaryDTO> lessons;

            if (withDetails)
            {
                lessons = await _lessonService.GetLessonsDetailsByFilter(lessonFilter, skip, take, cancellationToken);
            }
            else
            {
                lessons = await _lessonService.GetLessonsSummariesByFilter(lessonFilter, skip, take, cancellationToken);
            }

            return Ok(lessons);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LessonReadSummaryDTO>> GetLessonById(
            [FromRoute] int id,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            LessonReadSummaryDTO lesson;

            if (withDetails)
            {
                lesson = await _lessonService.GetLessonDetailsById(id, cancellationToken);
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
