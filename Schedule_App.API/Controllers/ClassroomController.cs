using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces.Services;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class ClassroomController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/classrooms";

        private readonly IClassroomService _classroomService;

        public ClassroomController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassroomReadSummaryDTO>>> GetClassrooms(
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<ClassroomReadSummaryDTO> classrooms;

            if (includeAuditInfo)
            {
                classrooms = await _classroomService.GetClassroomsDetailed(offset, limit, cancellationToken);
            }
            else
            {
                classrooms = await _classroomService.GetClassroomsSummaries(offset, limit, cancellationToken);
            }

            return Ok(classrooms);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClassroomReadSummaryDTO>>> GetClassroomsByFilter(
            [FromQuery] string? title = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            var classroomFilter = new ClassroomFilter()
            {
                Title = title,
            };

            IEnumerable<ClassroomReadSummaryDTO> classrooms;

            if (includeAuditInfo)
            {
                classrooms = await _classroomService.GetClassroomsDetailedByFilter(classroomFilter, offset, limit, cancellationToken);
            }
            else
            {
                classrooms = await _classroomService.GetClassroomsSummariesByFilter(classroomFilter, offset, limit, cancellationToken);
            }

            return Ok(classrooms);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClassroomReadSummaryDTO>> GetClassroomById(
            [FromRoute] int id,
            [FromQuery] bool includeAuditInfo = false, 
            CancellationToken cancellationToken = default)
        {
            ClassroomReadSummaryDTO classroom;

            if (includeAuditInfo)
            {
                classroom = await _classroomService.GetClassroomDetailedById(id, cancellationToken);
            }
            else
            {
                classroom = await _classroomService.GetClassroomSummaryById(id, cancellationToken);
            }

            return Ok(classroom);
        }

        [HttpPost]
        public async Task<ActionResult<ClassroomReadSummaryDTO>> AddClassroom([FromBody] ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken)
        {
            var classroom = await _classroomService.AddClassroom(classroomCreateDTO, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{classroom.Id}", classroom);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteClassroom(int id, CancellationToken cancellationToken)
        {
            await _classroomService.DeleteClassroom(id, cancellationToken);

            return NoContent();
        }
    }
}
