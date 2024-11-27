using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/teachers";
        private readonly ITeacherService _teacherService;
        private readonly ITeacherSubjectService _teacherSubjectService;

        public TeacherController(ITeacherService teacherService, ITeacherSubjectService teacherSubjectService)
        {
            _teacherService = teacherService;
            _teacherSubjectService = teacherSubjectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherReadSummaryDTO>>> GetTeachers(
            [FromQuery] int skip = 0, 
            [FromQuery] int take = 20,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<TeacherReadSummaryDTO> teachers;

            if (withDetails)
            {
                teachers = await _teacherService.GetTeachersDetails(skip, take, cancellationToken);
            }
            else
            {
                teachers = await _teacherService.GetTeachersSummaries(skip, take, cancellationToken);
            }

            return Ok(teachers);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TeacherReadSummaryDTO>>> GetTeachersByFilter(
            [FromQuery] string? username = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            var teacherFilter = new TeacherFilter()
            {
                Username = username,
                GroupId = groupId,
                SubjectId = subjectId,
            };

            IEnumerable<TeacherReadSummaryDTO> teachers;

            if (withDetails)
            {
                teachers = await _teacherService.GetTeachersDetailsByFilter(teacherFilter, skip, take, cancellationToken);
            }
            else
            {
                teachers = await _teacherService.GetTeachersSummariesByFilter(teacherFilter, skip, take, cancellationToken);
            }

            return Ok(teachers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeacherReadSummaryDTO>> GetTeacherById(
            [FromRoute] int id,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            TeacherReadSummaryDTO teacher;

            if (withDetails)
            {
                teacher = await _teacherService.GetTeacherDetailsById(id, cancellationToken);
            }
            else
            {
                teacher = await _teacherService.GetTeacherSummaryById(id, cancellationToken);
            }

            return Ok(teacher);
        }

        [HttpPost]
        public async Task<ActionResult<TeacherReadSummaryDTO>> AddTeacher([FromBody] TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken)
        {
            var result = await _teacherService.AddTeacher(teacherCreateDTO, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }

        [HttpPost("{teacherId:int}/subject/{subjectId:int}")]
        public async Task<ActionResult> AddSubjectToTeacher([FromRoute] int teacherId, [FromRoute] int subjectId, CancellationToken cancellationToken)
        {
            await _teacherSubjectService.AddSubjectToTeacher(teacherId, subjectId, cancellationToken);

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<TeacherReadSummaryDTO>> UpdateTeacher([FromRoute] int id, [FromBody] TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken)
        {
            var result = await _teacherService.UpdateTeacher(id, teacherUpdateDTO, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTeacher(int id, CancellationToken cancellationToken)
        {
            await _teacherService.DeleteTeacher(id, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{teacherId:int}/subject/{subjectId:int}")]
        public async Task<ActionResult> RemoveSubjectFromTeacher([FromRoute] int teacherId, [FromRoute] int subjectId, CancellationToken cancellationToken)
        {
            await _teacherSubjectService.RemoveSubjectFromTeacher(teacherId, subjectId, cancellationToken);

            return NoContent();
        }
    }
}
