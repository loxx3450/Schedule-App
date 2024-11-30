using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class SubjectController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/subjects";

        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public SubjectController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubjectReadSummaryDTO>>> GetSubjects(
            [FromQuery] string? title = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            var subjectFilter = new SubjectFilter()
            {
                Title = title,
                TeacherId = teacherId,
            };

            Subject[] subjects = await _subjectService.GetSubjects(subjectFilter, offset, limit, cancellationToken);

            IEnumerable<SubjectReadSummaryDTO> result;

            if (includeAuditInfo)
                result = _mapper.Map<IEnumerable<SubjectReadFullDTO>>(subjects);
            else
                result = _mapper.Map<IEnumerable<SubjectReadSummaryDTO>>(subjects);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubjectReadSummaryDTO>> GetSubjectById(
            [FromRoute] int id,
            [FromQuery] bool includeAuditInfo = false, 
            CancellationToken cancellationToken = default)
        {
            Subject subject = await _subjectService.GetSubjectById(id, cancellationToken);

            SubjectReadSummaryDTO result;

            if (includeAuditInfo)
                result = _mapper.Map<SubjectReadFullDTO>(subject);
            else
                result = _mapper.Map<SubjectReadSummaryDTO>(subject);

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SubjectReadSummaryDTO>> AddSubject([FromBody] SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken)
        {
            var subject = _mapper.Map<Subject>(subjectCreateDTO);

            Subject createdSubject = await _subjectService.AddSubject(subject, cancellationToken);

            var result = _mapper.Map<SubjectReadSummaryDTO>(createdSubject);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SubjectReadSummaryDTO>> UpdateSubjectTitle(
            [FromRoute] int id,
            [FromBody] SubjectUpdateDTO subjectUpdateDTO,
            CancellationToken cancellationToken)
        {
            Subject subject = await _subjectService.UpdateSubject(id, subjectUpdateDTO, cancellationToken);

            var result = _mapper.Map<SubjectReadSummaryDTO>(subject);

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
