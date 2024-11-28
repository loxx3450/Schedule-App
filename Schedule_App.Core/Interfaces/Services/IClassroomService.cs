using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface IClassroomService
    {
        Task<IEnumerable<ClassroomReadSummaryDTO>> GetClassroomsSummaries(int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<ClassroomReadFullDTO>> GetClassroomsDetailed(int offset, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<ClassroomReadSummaryDTO>> GetClassroomsSummariesByFilter(ClassroomFilter filter, int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<ClassroomReadFullDTO>> GetClassroomsDetailedByFilter(ClassroomFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<ClassroomReadSummaryDTO> GetClassroomSummaryById(int id, CancellationToken cancellationToken);
        Task<ClassroomReadFullDTO> GetClassroomDetailedById(int id, CancellationToken cancellationToken);

        Task<ClassroomReadSummaryDTO> AddClassroom(ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken);

        Task DeleteClassroom(int id, CancellationToken cancellationToken);
    }
}
