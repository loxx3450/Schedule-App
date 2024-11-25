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
        Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummaries(int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetails(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummariesByFilter(SubjectFilter filter, int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetailsByFilter(SubjectFilter filter, int skip, int take, CancellationToken cancellationToken);

        Task<SubjectReadSummaryDTO> GetSubjectSummaryById(int id, CancellationToken cancellationToken);
        Task<SubjectReadFullDTO> GetSubjectDetailsById(int id, CancellationToken cancellationToken);

        Task<SubjectReadSummaryDTO> AddSubject(SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken);

        Task<SubjectReadSummaryDTO> UpdateSubjectTitle(int id, SubjectUpdateDTO subjectUpdateDTO, CancellationToken cancellationToken);

        Task DeleteSubject(int id, CancellationToken cancellationToken);
    }
}
