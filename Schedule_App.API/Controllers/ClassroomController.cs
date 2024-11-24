using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    public class ClassroomController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/classrooms";
        private readonly IClassroomService _classroomService;

        public ClassroomController(IClassroomService classroomService)
        {
            _classroomService = classroomService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassroomReadDTO>>> GetClassrooms(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var classrooms = await _classroomService.GetClassrooms(skip, take, cancellationToken);

            return Ok(classrooms);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ClassroomReadDTO>>> GetClassroomsByFilter(
            [FromQuery] string? title = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var classroomFilter = new ClassroomFilter()
            {
                Title = title,
            };

            var classrooms = await _classroomService.GetClassroomsByFilter(classroomFilter, skip, take, cancellationToken);

            return Ok(classrooms);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClassroomReadDTO>> GetClassroomById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var classroom = await _classroomService.GetClassroomById(id, cancellationToken);

            return Ok(classroom);
        }

        [HttpPost]
        public async Task<ActionResult<ClassroomReadDTO>> AddClassroom([FromBody] ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken)
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
