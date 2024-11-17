using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route("groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupReadDTO>>> GetGroups(
            [FromQuery] string? title = null, 
            [FromQuery] int skip = 0, 
            [FromQuery] int take = 20, 
            CancellationToken cancellationToken = default)
        {
            IEnumerable<GroupReadDTO> result = [];

            if (title is null)
            {
                result = await _groupService.GetGroups(skip, take, cancellationToken);
            }
            else
            {
                result = await _groupService.GetGroupsByTitlePattern(title, skip, take, cancellationToken);
            }

            return Ok(result);
        }

        [HttpGet("search/teacher/{id:int}")]
        public async Task<ActionResult<IEnumerable<GroupReadDTO>>> GetGroupsByTeacherId(
            [FromRoute] int id, 
            [FromQuery] int skip = 0, 
            [FromQuery] int take = 20, 
            CancellationToken cancellationToken = default)
        {
            var result = await _groupService.GetGroupsByTeacherId(id, skip, take, cancellationToken);

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

            return Created($"groups/{result.Id}", result);
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult<GroupReadDTO>> UpdateGroupTitle([FromRoute] int id, [FromQuery] string title, CancellationToken cancellationToken)
        {
            var result = await _groupService.UpdateGroupTitle(id, title, cancellationToken);

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
