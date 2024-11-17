using Schedule_App.Core.DTOs.Group;
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

        Task<IEnumerable<GroupReadDTO>> GetGroupsByTitlePattern(string title, int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<GroupReadDTO>> GetGroupsByTeacherId(int id, int skip, int take, CancellationToken cancellationToken);

        Task<GroupReadDTO> GetGroupById(int id, CancellationToken cancellationToken);

        Task<GroupReadDTO> GetGroupByTitle(string title, CancellationToken cancellationToken);

        Task<GroupReadDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken);

        Task<GroupReadDTO> UpdateGroupTitle(int id, string newTitle, CancellationToken cancellationToken);

        Task DeleteGroup(int id, CancellationToken cancellationToken);
    }
}
