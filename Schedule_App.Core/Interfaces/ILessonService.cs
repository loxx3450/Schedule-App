using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonReadSummaryDTO>> GetLessonsSummaries(int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<LessonReadFullDTO>> GetLessonsDetails(int skip, int take, CancellationToken cancellationToken);

        Task<IEnumerable<LessonReadSummaryDTO>> GetLessonsSummariesByFilter(LessonFilter filter, int skip, int take, CancellationToken cancellationToken);
        Task<IEnumerable<LessonReadFullDTO>> GetLessonsDetailsByFilter(LessonFilter filter, int skip, int take, CancellationToken cancellationToken);

        Task<LessonReadSummaryDTO> GetLessonSummaryById(int id, CancellationToken cancellationToken);
        Task<LessonReadFullDTO> GetLessonDetailsById(int id, CancellationToken cancellationToken);
    }
}
