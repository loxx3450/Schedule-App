using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface ISubjectService
    {
        Task<Subject[]> GetSubjects(SubjectFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<Subject> GetSubjectById(int id, CancellationToken cancellationToken);

        Task<Subject> AddSubject(Subject subject, CancellationToken cancellationToken);

        Task<Subject> UpdateSubject(int id, SubjectUpdateDTO subjectUpdateDTO, CancellationToken cancellationToken);

        Task DeleteSubject(int id, CancellationToken cancellationToken);
    }
}
