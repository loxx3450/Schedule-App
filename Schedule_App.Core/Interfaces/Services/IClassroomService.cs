using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface IClassroomService
    {
        Task<Classroom[]> GetClassrooms(ClassroomFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<Classroom> GetClassroomById(int id, CancellationToken cancellationToken);

        Task<Classroom> AddClassroom(Classroom classroomCreateDTO, CancellationToken cancellationToken);

        Task DeleteClassroom(int id, CancellationToken cancellationToken);
    }
}
