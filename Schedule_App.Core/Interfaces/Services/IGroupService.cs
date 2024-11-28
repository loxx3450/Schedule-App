using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupReadSummaryDTO>> GetGroupsSummaries(int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<GroupReadFullDTO>> GetGroupsDetailed(int offset, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<GroupReadSummaryDTO>> GetGroupsSummariesByFilter(GroupFilter groupFilter, int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<GroupReadFullDTO>> GetGroupsDetailedByFilter(GroupFilter groupFilter, int offset, int limit, CancellationToken cancellationToken);

        Task<GroupReadSummaryDTO> GetGroupSummaryById(int id, CancellationToken cancellationToken);
        Task<GroupReadFullDTO> GetGroupDetailedById(int id, CancellationToken cancellationToken);

        Task<GroupReadSummaryDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken);

        Task<GroupReadSummaryDTO> UpdateGroup(int id, GroupUpdateDTO groupUpdateDTO, CancellationToken cancellationToken);

        Task DeleteGroup(int id, CancellationToken cancellationToken);
    }
}
