using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface ITeacherService
    {
        Task<Teacher[]> GetTeachers(TeacherFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<Teacher> GetTeacherById(int id, CancellationToken cancellationToken);

        Task<Teacher> AddTeacher(Teacher teacher, CancellationToken cancellationToken);

        Task<Teacher> UpdateTeacher(int id, TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken);

        Task DeleteTeacher(int id, CancellationToken cancellationToken);
    }
}
