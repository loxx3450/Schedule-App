using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class ClassroomController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/classrooms";

        private readonly IClassroomService _classroomService;
        private readonly IMapper _mapper;

        public ClassroomController(IClassroomService classroomService, IMapper mapper)
        {
            _classroomService = classroomService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClassroomReadSummaryDTO>>> GetClassrooms(
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

            Classroom[] classrooms = await _classroomService.GetClassrooms(classroomFilter, offset, limit, cancellationToken);

            IEnumerable<ClassroomReadSummaryDTO> result;

            if (includeAuditInfo)
                result = _mapper.Map<IEnumerable<ClassroomReadFullDTO>>(classrooms);
            else
                result = _mapper.Map<IEnumerable<ClassroomReadSummaryDTO>>(classrooms);

            return Ok(result);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClassroomReadSummaryDTO>> GetClassroomById(
            [FromRoute] int id,
            [FromQuery] bool includeAuditInfo = false, 
            CancellationToken cancellationToken = default)
        {
            Classroom classroom = await _classroomService.GetClassroomById(id, cancellationToken);

            ClassroomReadSummaryDTO result;

            if (includeAuditInfo)
                result = _mapper.Map<ClassroomReadFullDTO>(classroom);
            else
                result = _mapper.Map<ClassroomReadSummaryDTO>(classroom);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ClassroomReadSummaryDTO>> AddClassroom([FromBody] ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken)
        {
            var classroom = _mapper.Map<Classroom>(classroomCreateDTO);

            Classroom createdClassroom = await _classroomService.AddClassroom(classroom, cancellationToken);

            var result = _mapper.Map<ClassroomReadSummaryDTO>(createdClassroom);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteClassroom(int id, CancellationToken cancellationToken)
        {
            await _classroomService.DeleteClassroom(id, cancellationToken);

            return NoContent();
        }
    }
}
