using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

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
        private readonly IMapper _mapper;

        public TeacherController(ITeacherService teacherService, ITeacherSubjectService teacherSubjectService, IMapper mapper)
        {
            _teacherService = teacherService;
            _teacherSubjectService = teacherSubjectService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TeacherReadSummaryDTO>>> GetTeachers(
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

            Teacher[] teachers = await _teacherService.GetTeachers(teacherFilter, offset, limit, cancellationToken);

            IEnumerable<TeacherReadSummaryDTO> result;

            if (includeAuditInfo)
                result = _mapper.Map<IEnumerable<TeacherReadFullDTO>>(teachers);
            else
                result = _mapper.Map<IEnumerable<TeacherReadSummaryDTO>>(teachers);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TeacherReadSummaryDTO>> GetTeacherById(
            [FromRoute] int id,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            Teacher teacher = await _teacherService.GetTeacherById(id, cancellationToken);

            TeacherReadSummaryDTO result;

            if (includeAuditInfo)
                result = _mapper.Map<TeacherReadFullDTO>(teacher);
            else
                result = _mapper.Map<TeacherReadSummaryDTO>(teacher);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<TeacherReadSummaryDTO>> AddTeacher([FromBody] TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken)
        {
            var teacher = _mapper.Map<Teacher>(teacherCreateDTO);

            Teacher createdTeacher = await _teacherService.AddTeacher(teacher, cancellationToken);

            var result = _mapper.Map<TeacherReadSummaryDTO>(teacher);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
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
            Teacher teacher = await _teacherService.UpdateTeacher(id, teacherUpdateDTO, cancellationToken);

            var result = _mapper.Map<TeacherReadSummaryDTO>(teacher);

            return Ok(result);
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
