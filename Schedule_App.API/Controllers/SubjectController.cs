using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    public class SubjectController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/subjects";
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectReadDTO>>> GetSubjects(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _subjectService.GetSubjects(skip, take, cancellationToken);

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SubjectReadDTO>>> GetSubjectsByFilter(
            [FromQuery] string? title = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var subjectFilter = new SubjectFilter()
            {
                Title = title,
                TeacherId = teacherId,
            };

            var result = await _subjectService.GetSubjectsByFilter(subjectFilter, skip, take, cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubjectReadDTO>> GetSubjectById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _subjectService.GetSubjectById(id, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SubjectReadDTO>> AddGroup([FromBody] SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken)
        {
            var result = await _subjectService.AddSubject(subjectCreateDTO, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SubjectReadDTO>> UpdateSubjectTitle(
            [FromRoute] int id,
            [FromBody] SubjectUpdateDTO subjectUpdateDTO,
            CancellationToken cancellationToken)
        {
            var result = await _subjectService.UpdateSubjectTitle(id, subjectUpdateDTO, cancellationToken);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteSubject(int id, CancellationToken cancellationToken)
        {
            await _subjectService.DeleteSubject(id, cancellationToken);

            return NoContent();
        }
    }
}
