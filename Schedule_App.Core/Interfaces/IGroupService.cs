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

        Task<IEnumerable<GroupReadDTO>> GetGroupsByTitle(string title, int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<GroupReadDTO>> GetGroupsByTeacherId(int id, int skip, int take, CancellationToken cancellationToken);

        Task<GroupReadDTO> GetGroupById(int id, CancellationToken cancellationToken);

        Task<GroupReadDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken);

        Task DeleteGroup(int id, CancellationToken cancellationToken);
    }
}
