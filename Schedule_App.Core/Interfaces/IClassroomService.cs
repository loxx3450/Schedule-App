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
        Task<IEnumerable<ClassroomReadDTO>> GetClassrooms(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<ClassroomReadDTO>> GetClassroomsByFilter(ClassroomFilter filter, int skip, int take, CancellationToken cancellationToken);

        Task<ClassroomReadDTO> GetClassroomById(int id, CancellationToken cancellationToken);

        Task<ClassroomReadDTO> AddClassroom(ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken);

        Task DeleteClassroom(int id, CancellationToken cancellationToken);
    }
}
