using Schedule_App.Core.DTOs.GroupTeacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface IGroupTeacherService
    {
        Task<IEnumerable<GroupTeacherReadDTO>> GetGroupTeacherAssociations(int offset, int limit, CancellationToken cancellationToken);

        Task<GroupTeacherReadDTO> GetGroupTeacherAssociation(int groupId, int teacherId, CancellationToken cancellationToken);

        Task<IEnumerable<GroupTeacherReadDTO>> GetGroupsByTeacherId(int teacherId, int offset, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<GroupTeacherReadDTO>> GetTeachersByGroupId(int groupId, int offset, int limit, CancellationToken cancellationToken);

        Task AddTeacherToGroup(GroupTeacherCreateDTO createDTO, CancellationToken cancellationToken);

        Task RemoveTeacherFromGroup(int groupId, int teacherId, CancellationToken cancellationToken);
    }
}
