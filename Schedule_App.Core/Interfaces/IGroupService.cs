using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface IGroupService
    {
        Task<IEnumerable<GroupReadSummaryDTO>> GetGroupsSummaries(int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<GroupReadFullDTO>> GetGroupsDetails(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<GroupReadSummaryDTO>> GetGroupsSummariesByFilter(GroupFilter groupFilter, int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<GroupReadFullDTO>> GetGroupsDetailsByFilter(GroupFilter groupFilter, int skip, int take, CancellationToken cancellationToken);

        Task<GroupReadSummaryDTO> GetGroupSummaryById(int id, CancellationToken cancellationToken);
        Task<GroupReadFullDTO> GetGroupDetailsById(int id, CancellationToken cancellationToken);

        Task<GroupReadSummaryDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken);

        Task<GroupReadSummaryDTO> UpdateGroupTitle(int id, GroupUpdateDTO groupUpdateDTO, CancellationToken cancellationToken);

        Task DeleteGroup(int id, CancellationToken cancellationToken);
    }
}
