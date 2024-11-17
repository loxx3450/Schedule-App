using Schedule_App.Core.DTOs.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface ITeacherService
    {
        Task<IEnumerable<TeacherReadDTO>> GetTeachers(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<TeacherReadDTO>> GetTeachersByGroupId(int groupId, int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<TeacherReadDTO>> GetTeachersBySubjectId(int subjectId, int skip, int take, CancellationToken cancellationToken);

        Task<TeacherReadDTO> GetTeacherById(int id, CancellationToken cancellationToken);

        Task<TeacherReadDTO> GetTeacherByUsername(string username, CancellationToken cancellationToken);

        Task<TeacherReadDTO> AddTeacher(TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken);

        Task<TeacherReadDTO> UpdateTeacher(int id, TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken);

        Task DeleteTeacher(int id, CancellationToken cancellationToken);
    }
}
