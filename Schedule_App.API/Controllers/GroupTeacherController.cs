using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.Interfaces.Services;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class GroupTeacherController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/groups_teachers";
        private readonly IGroupTeacherService _groupTeacherService;

        public GroupTeacherController(IGroupTeacherService groupTeacherService)
        {
            _groupTeacherService = groupTeacherService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupTeacherReadDTO>>> GetGroupTeacherInfos(
            [FromQuery] int? groupId = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<GroupTeacherReadDTO> result;

            if (groupId.HasValue && teacherId.HasValue)
            {
                // Get the exact one match
                result = [ await _groupTeacherService.GetGroupTeacherAssociation(groupId.Value!, teacherId.Value!, cancellationToken) ];
            }
            else if (teacherId.HasValue)
            {
                // Get all Groups that have association with Teacher + extra info about their relationship
                result = await _groupTeacherService.GetGroupsByTeacherId(teacherId.Value!, offset, limit, cancellationToken);
            }
            else if (groupId.HasValue)
            {
                // Get all Teachers that have association with Group + extra info about their relationship
                result = await _groupTeacherService.GetTeachersByGroupId(groupId.Value!, offset, limit, cancellationToken);
            }
            else
            {
                // Get all GroupTeacher's
                result = await _groupTeacherService.GetGroupTeacherAssociations(offset, limit, cancellationToken);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> AddTeacherToGroup(
            [FromBody] GroupTeacherCreateDTO createDTO, 
            CancellationToken cancellationToken)
        {
            await _groupTeacherService.AddTeacherToGroup(createDTO, cancellationToken);

            return NoContent();
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveTeacherFromGroup(
            [FromQuery] int groupId,
            [FromQuery] int teacherId,
            CancellationToken cancellationToken)
        {
            await _groupTeacherService.RemoveTeacherFromGroup(groupId, teacherId, cancellationToken);

            return NoContent();
        }
    }
}
