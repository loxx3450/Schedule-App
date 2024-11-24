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
        Task<IEnumerable<GroupReadDTO>> GetGroups(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<GroupReadDTO>> GetGroupsByFilter(GroupFilter groupFilter, int skip, int take, CancellationToken cancellationToken);

        Task<GroupReadDTO> GetGroupById(int id, CancellationToken cancellationToken);

        Task<GroupReadDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken);

        Task<GroupReadDTO> UpdateGroupTitle(int id, GroupUpdateDTO groupUpdateDTO, CancellationToken cancellationToken);

        Task DeleteGroup(int id, CancellationToken cancellationToken);
    }
}
