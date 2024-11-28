using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces.Services;

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
            [FromQuery] int offset = 0, 
            [FromQuery] int limit = 20,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<TeacherReadSummaryDTO> teachers;

            if (includeAuditInfo)
            {
                teachers = await _teacherService.GetTeachersDetailed(offset, limit, cancellationToken);
            }
            else
            {
                teachers = await _teacherService.GetTeachersSummaries(offset, limit, cancellationToken);
            }

            return Ok(teachers);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TeacherReadSummaryDTO>>> GetTeachersByFilter(
            [FromQuery] string? username = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            var teacherFilter = new TeacherFilter()
            {
                Username = username,
                GroupId = groupId,
                SubjectId = subjectId,
            };

            IEnumerable<TeacherReadSummaryDTO> teachers;

            if (includeAuditInfo)
            {
                teachers = await _teacherService.GetTeachersDetailedByFilter(teacherFilter, offset, limit, cancellationToken);
            }
            else
            {
                teachers = await _teacherService.GetTeachersSummariesByFilter(teacherFilter, offset, limit, cancellationToken);
            }

            return Ok(teachers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeacherReadSummaryDTO>> GetTeacherById(
            [FromRoute] int id,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            TeacherReadSummaryDTO teacher;

            if (includeAuditInfo)
            {
                teacher = await _teacherService.GetTeacherDetailedById(id, cancellationToken);
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
            var teacher = await _teacherService.AddTeacher(teacherCreateDTO, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{teacher.Id}", teacher);
        }

        [HttpPost("{teacherId:int}/subject/{subjectId:int}")]
        public async Task<ActionResult> AddSubjectToTeacher(
            [FromRoute] int teacherId, 
            [FromRoute] int subjectId, 
            CancellationToken cancellationToken)
        {
            await _teacherSubjectService.AddSubjectToTeacher(teacherId, subjectId, cancellationToken);

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<TeacherReadSummaryDTO>> UpdateTeacher(
            [FromRoute] int id, 
            [FromBody] TeacherUpdateDTO teacherUpdateDTO, 
            CancellationToken cancellationToken)
        {
            var teacher = await _teacherService.UpdateTeacher(id, teacherUpdateDTO, cancellationToken);

            return Ok(teacher);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTeacher(int id, CancellationToken cancellationToken)
        {
            await _teacherService.DeleteTeacher(id, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{teacherId:int}/subject/{subjectId:int}")]
        public async Task<ActionResult> RemoveSubjectFromTeacher(
            [FromRoute] int teacherId, 
            [FromRoute] int subjectId, 
            CancellationToken cancellationToken)
        {
            await _teacherSubjectService.RemoveSubjectFromTeacher(teacherId, subjectId, cancellationToken);

            return NoContent();
        }
    }
}
