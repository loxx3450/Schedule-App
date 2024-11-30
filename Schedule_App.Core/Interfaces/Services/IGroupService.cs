using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface IGroupService
    {
        Task<Group[]> GetGroups(GroupFilter groupFilter, int offset, int limit, CancellationToken cancellationToken);

        Task<Group> GetGroupById(int id, CancellationToken cancellationToken);

        Task<Group> AddGroup(Group group, CancellationToken cancellationToken);

        Task<Group> UpdateGroup(int id, GroupUpdateDTO groupUpdateDTO, CancellationToken cancellationToken);

        Task DeleteGroup(int id, CancellationToken cancellationToken);
    }
}
