using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface ILessonService
    {
        Task<IEnumerable<LessonReadSummaryDTO>> GetLessonsSummaries(int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<LessonReadFullDTO>> GetLessonsDetailed(int offset, int limit, CancellationToken cancellationToken);

        Task<IEnumerable<LessonReadSummaryDTO>> GetLessonsSummariesByFilter(LessonFilter filter, int offset, int limit, CancellationToken cancellationToken);
        Task<IEnumerable<LessonReadFullDTO>> GetLessonsDetailedByFilter(LessonFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<LessonReadSummaryDTO> GetLessonSummaryById(int id, CancellationToken cancellationToken);
        Task<LessonReadFullDTO> GetLessonDetailedById(int id, CancellationToken cancellationToken);

        Task<LessonReadSummaryDTO> AddLesson(LessonCreateDTO lessonCreateDTO, CancellationToken cancellationToken);

        Task<LessonReadSummaryDTO> UpdateLesson(int id, LessonUpdateDTO lessonUpdateDTO, CancellationToken cancellationToken);

        Task DeleteLesson(int id, CancellationToken cancellationToken);
    }
}
