using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface ITeacherSubjectService
    {
        Task AddSubjectToTeacher(int teacherId, int subjectId, CancellationToken cancellationToken);

        Task RemoveSubjectFromTeacher(int teacherId, int subjectId, CancellationToken cancellationToken);
    }
}
