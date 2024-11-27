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
        Task<IEnumerable<TeacherReadSummaryDTO>> GetTeachersSummaries(int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<TeacherReadFullDTO>> GetTeachersDetailed(int offset, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<TeacherReadSummaryDTO>> GetTeachersSummariesByFilter(TeacherFilter filter, int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<TeacherReadFullDTO>> GetTeachersDetailedByFilter(TeacherFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<TeacherReadSummaryDTO> GetTeacherSummaryById(int id, CancellationToken cancellationToken);
        Task<TeacherReadFullDTO> GetTeacherDetailedById(int id, CancellationToken cancellationToken);

        Task<TeacherReadSummaryDTO> AddTeacher(TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken);

        Task<TeacherReadSummaryDTO> UpdateTeacher(int id, TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken);

        Task DeleteTeacher(int id, CancellationToken cancellationToken);
    }
}
