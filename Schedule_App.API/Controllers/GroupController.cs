using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;
using System.Collections.Generic;

namespace Schedule_App.API.Controllers
{
    [ApiController]
    [Route(BASE_ENDPOINT)]
    [Authorize]
    public class GroupController : ControllerBase
    {
        private const string BASE_ENDPOINT = "api/groups";

        private readonly IGroupService _groupService;
        private readonly IMapper _mapper;

        public GroupController(IGroupService groupService, IMapper mapper)
        {
            _groupService = groupService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupReadSummaryDTO>>> GetGroups(
            [FromQuery] string? title = null,
            [FromQuery] string? titlePattern = null,
            [FromQuery] int? teacherId = null,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 20,
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            var groupFilter = new GroupFilter()
            {
                Title = title,
                TitlePattern = titlePattern,
                TeacherId = teacherId,
            };

            Group[] groups = await _groupService.GetGroups(groupFilter, offset, limit, cancellationToken);

            IEnumerable<GroupReadSummaryDTO> result;

            if (includeAuditInfo)
                result = _mapper.Map<IEnumerable<GroupReadFullDTO>>(groups);
            else
                result = _mapper.Map<IEnumerable<GroupReadSummaryDTO>>(groups);

            return Ok(result);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<GroupReadSummaryDTO>> GetGroupById(
            [FromRoute] int id, 
            [FromQuery] bool includeAuditInfo = false,
            CancellationToken cancellationToken = default)
        {
            Group group = await _groupService.GetGroupById(id, cancellationToken);

            GroupReadSummaryDTO result;

            if (includeAuditInfo)
                result = _mapper.Map<GroupReadFullDTO>(group);
            else
                result = _mapper.Map<GroupReadSummaryDTO>(group);

            return Ok(result);
        }


        [HttpPost]
        public async Task<ActionResult<GroupReadSummaryDTO>> AddGroup([FromBody] GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken)
        {
            var group = _mapper.Map<Group>(groupCreateDTO);

            Group createdGroup = await _groupService.AddGroup(group, cancellationToken);

            var result = _mapper.Map<GroupReadSummaryDTO>(createdGroup);

            return Created($"{BASE_ENDPOINT}/{result.Id}", result);
        }


        [HttpPatch("{id:int}")]
        public async Task<ActionResult<GroupReadSummaryDTO>> UpdateGroupTitle(
            [FromRoute] int id, 
            [FromBody] GroupUpdateDTO groupUpdateDTO, 
            CancellationToken cancellationToken)
        {
            Group group = await _groupService.UpdateGroup(id, groupUpdateDTO, cancellationToken);

            var result = _mapper.Map<GroupReadSummaryDTO>(group);

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
