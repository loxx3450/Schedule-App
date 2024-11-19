using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
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

        Task<IEnumerable<TeacherReadDTO>> GetTeachersByFilter(TeacherFilter filter, int skip, int take, CancellationToken cancellationToken);

        Task<TeacherReadDTO> GetTeacherById(int id, CancellationToken cancellationToken);

        Task<TeacherReadDTO> AddTeacher(TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken);

        Task<TeacherReadDTO> UpdateTeacher(int id, TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken);

        Task DeleteTeacher(int id, CancellationToken cancellationToken);
    }
}
