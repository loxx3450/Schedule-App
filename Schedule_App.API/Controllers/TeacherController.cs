using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
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
        public async Task<ActionResult<IEnumerable<TeacherReadDTO>>> GetTeachers(
            [FromQuery] int skip = 0, 
            [FromQuery] int take = 20, 
            CancellationToken cancellationToken = default)
        {
            var result = await _teacherService.GetTeachers(skip, take, cancellationToken);

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TeacherReadDTO>>> GetTeachersByFilter(
            [FromQuery] string? username = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int? subjectId = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var teacherFilter = new TeacherFilter()
            {
                Username = username,
                GroupId = groupId,
                SubjectId = subjectId,
            };

            var result = await _teacherService.GetTeachersByFilter(teacherFilter, skip, take, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeacherReadDTO>> GetTeacherById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _teacherService.GetTeacherById(id, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<TeacherReadDTO>> AddTeacher([FromBody] TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken)
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
        public async Task<ActionResult<TeacherReadDTO>> UpdateTeacher([FromRoute] int id, [FromBody] TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken)
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
