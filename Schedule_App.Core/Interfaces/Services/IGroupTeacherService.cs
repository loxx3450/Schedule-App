using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface IGroupTeacherService
    {
        Task<GroupTeacher[]> GetGroupTeacherAssociations(int offset, int limit, CancellationToken cancellationToken);

        Task<GroupTeacher> GetGroupTeacherAssociation(int groupId, int teacherId, CancellationToken cancellationToken);

        Task<GroupTeacher[]> GetGroupsByTeacherId(int teacherId, int offset, int limit, CancellationToken cancellationToken);

        Task<GroupTeacher[]> GetTeachersByGroupId(int groupId, int offset, int limit, CancellationToken cancellationToken);

        Task AddTeacherToGroup(GroupTeacher groupTeacher, CancellationToken cancellationToken);

        Task RemoveTeacherFromGroup(int groupId, int teacherId, CancellationToken cancellationToken);
    }
}
