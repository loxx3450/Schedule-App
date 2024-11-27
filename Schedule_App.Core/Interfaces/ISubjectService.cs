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
        Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummaries(int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetailed(int offset, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummariesByFilter(SubjectFilter filter, int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetailedByFilter(SubjectFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<SubjectReadSummaryDTO> GetSubjectSummaryById(int id, CancellationToken cancellationToken);
        Task<SubjectReadFullDTO> GetSubjectDetailedById(int id, CancellationToken cancellationToken);

        Task<SubjectReadSummaryDTO> AddSubject(SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken);

        Task<SubjectReadSummaryDTO> UpdateSubject(int id, SubjectUpdateDTO subjectUpdateDTO, CancellationToken cancellationToken);

        Task DeleteSubject(int id, CancellationToken cancellationToken);
    }
}
