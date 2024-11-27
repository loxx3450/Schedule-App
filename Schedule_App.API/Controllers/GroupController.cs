using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/groups";
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupReadSummaryDTO>>> GetGroups(
            [FromQuery] int offset = 0, 
            [FromQuery] int limit = 20,
            [FromQuery] bool withDetailed = false,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<GroupReadSummaryDTO> groups;

            if (withDetailed)
            {
                groups = await _groupService.GetGroupsDetailed(offset, limit, cancellationToken);
            }
            else
            {
                groups = await _groupService.GetGroupsSummaries(offset, limit, cancellationToken);
            }

            return Ok(groups);
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GroupReadSummaryDTO>>> GetGroupsByFilter(
            [FromQuery] string? title = null,
            [FromQuery] string? titlePattern = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool withDetailed = false,
            CancellationToken cancellationToken = default)
        {
            var groupFilter = new GroupFilter()
            {
                Title = title,
                TitlePattern = titlePattern,
                TeacherId = teacherId,
            };

            IEnumerable<GroupReadSummaryDTO> groups;

            if (withDetailed)
            {
                groups = await _groupService.GetGroupsDetailedByFilter(groupFilter, offset, limit, cancellationToken);
            }
            else
            {
                groups = await _groupService.GetGroupsSummariesByFilter(groupFilter, offset, limit, cancellationToken);
            }

            return Ok(groups);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupReadSummaryDTO>> GetGroupById(
            [FromRoute] int id, 
            [FromQuery] bool withDetailed = false,
            CancellationToken cancellationToken = default)
        {
            GroupReadSummaryDTO group;

            if (withDetailed)
            {
                group = await _groupService.GetGroupDetailedById(id, cancellationToken);
            }
            else
            {
                group = await _groupService.GetGroupSummaryById(id, cancellationToken);
            }

            return Ok(group);
        }


        [HttpPost]
        public async Task<ActionResult<GroupReadSummaryDTO>> AddGroup([FromBody] GroupCreateDTO group, CancellationToken cancellationToken)
        {
            var result = await _groupService.AddGroup(group, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }


        [HttpPatch("{id:int}")]
        public async Task<ActionResult<GroupReadSummaryDTO>> UpdateGroupTitle(
            [FromRoute] int id, 
            [FromBody] GroupUpdateDTO groupUpdateDTO, 
            CancellationToken cancellationToken)
        {
            var result = await _groupService.UpdateGroup(id, groupUpdateDTO, cancellationToken);

            return Ok(result);
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteGroup(int id, CancellationToken cancellationToken)
        {
            await _groupService.DeleteGroup(id, cancellationToken);

            return NoContent();
        }
    }
}
