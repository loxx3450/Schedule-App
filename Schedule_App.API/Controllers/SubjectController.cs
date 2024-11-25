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
        public async Task<ActionResult<IEnumerable<SubjectReadSummaryDTO>>> GetSubjects(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<SubjectReadSummaryDTO> subjects;

            if (withDetails)
            {
                subjects = await _subjectService.GetSubjectsDetails(skip, take, cancellationToken);
            }
            else
            {
                subjects = await _subjectService.GetSubjectsSummaries(skip, take, cancellationToken);
            }

            return Ok(subjects);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<SubjectReadSummaryDTO>>> GetSubjectsByFilter(
            [FromQuery] string? title = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            [FromQuery] bool withDetails = false,
            CancellationToken cancellationToken = default)
        {
            var subjectFilter = new SubjectFilter()
            {
                Title = title,
                TeacherId = teacherId,
            };

            IEnumerable<SubjectReadSummaryDTO> subjects;

            if (withDetails)
            {
                subjects = await _subjectService.GetSubjectsDetailsByFilter(subjectFilter, skip, take, cancellationToken);
            }
            else
            {
                subjects = await _subjectService.GetSubjectsSummariesByFilter(subjectFilter, skip, take, cancellationToken);
            }

            return Ok(subjects);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SubjectReadSummaryDTO>> GetSubjectById(
            [FromRoute] int id,
            [FromQuery] bool withDetails = false, 
            CancellationToken cancellationToken = default)
        {
            SubjectReadSummaryDTO subject;

            if (withDetails)
            {
                subject = await _subjectService.GetSubjectDetailsById(id, cancellationToken);
            }
            else
            {
                subject = await _subjectService.GetSubjectSummaryById(id, cancellationToken);
            }

            return Ok(subject);
        }

        [HttpPost]
        public async Task<ActionResult<SubjectReadSummaryDTO>> AddSubject([FromBody] SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken)
        {
            var result = await _subjectService.AddSubject(subjectCreateDTO, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SubjectReadSummaryDTO>> UpdateSubjectTitle(
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
