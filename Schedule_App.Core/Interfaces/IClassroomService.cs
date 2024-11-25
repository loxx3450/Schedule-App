using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface IClassroomService
    {
        Task<IEnumerable<ClassroomReadSummaryDTO>> GetClassroomsSummaries(int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<ClassroomReadFullDTO>> GetClassroomsDetails(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<ClassroomReadSummaryDTO>> GetClassroomsSummariesByFilter(ClassroomFilter filter, int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<ClassroomReadFullDTO>> GetClassroomsDetailsByFilter(ClassroomFilter filter, int skip, int take, CancellationToken cancellationToken);

        Task<ClassroomReadSummaryDTO> GetClassroomSummaryById(int id, CancellationToken cancellationToken);
        Task<ClassroomReadFullDTO> GetClassroomDetailsById(int id, CancellationToken cancellationToken);

        Task<ClassroomReadSummaryDTO> AddClassroom(ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken);

        Task DeleteClassroom(int id, CancellationToken cancellationToken);
    }
}
