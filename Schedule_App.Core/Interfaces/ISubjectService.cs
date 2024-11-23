using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectReadDTO>> GetSubjects(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<SubjectReadDTO>> GetSubjectsByFilter(SubjectFilter filter, int skip, int take, CancellationToken cancellationToken);

        Task<SubjectReadDTO> GetSubjectById(int id, CancellationToken cancellationToken);

        Task<SubjectReadDTO> AddSubject(SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken);

        Task<SubjectReadDTO> UpdateSubject(int id, string newTitle, CancellationToken cancellationToken);

        Task DeleteSubject(int id, CancellationToken cancellationToken);
    }
}
