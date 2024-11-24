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
    public class GroupController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/groups";
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupReadDTO>>> GetGroups(
            [FromQuery] int skip = 0, 
            [FromQuery] int take = 20, 
            CancellationToken cancellationToken = default)
        {
            var result = await _groupService.GetGroups(skip, take, cancellationToken);

            return Ok(result);
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GroupReadDTO>>> GetGroupsByFilter(
            [FromQuery] string? title = null,
            [FromQuery] string? titlePattern = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 20,
            CancellationToken cancellationToken = default)
        {
            var groupFilter = new GroupFilter()
            {
                Title = title,
                TitlePattern = titlePattern,
                TeacherId = teacherId,
            };

            var result = await _groupService.GetGroupsByFilter(groupFilter, skip, take, cancellationToken);

            return Ok(result);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupReadDTO>> GetGroupById([FromRoute] int id, CancellationToken cancellationToken)
        {
            var result = await _groupService.GetGroupById(id, cancellationToken);

            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult<GroupReadDTO>> AddGroup([FromBody] GroupCreateDTO group, CancellationToken cancellationToken)
        {
            var result = await _groupService.AddGroup(group, cancellationToken);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }


        [HttpPatch("{id:int}")]
        public async Task<ActionResult<GroupReadDTO>> UpdateGroupTitle(
            [FromRoute] int id, 
            [FromBody] GroupUpdateDTO groupUpdateDTO, 
            CancellationToken cancellationToken)
        {
            var result = await _groupService.UpdateGroupTitle(id, groupUpdateDTO, cancellationToken);

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
