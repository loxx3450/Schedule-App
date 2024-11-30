using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces.Services
{
    public interface ILessonService
    {
        Task<Lesson[]> GetLessons(LessonFilter filter, int offset, int limit, CancellationToken cancellationToken);

        Task<Lesson> GetLessonById(int id, CancellationToken cancellationToken);

        Task<Lesson> AddLesson(Lesson lesson, CancellationToken cancellationToken);

        Task<Lesson> UpdateLesson(int id, LessonUpdateDTO lessonUpdateDTO, CancellationToken cancellationToken);

        Task DeleteLesson(int id, CancellationToken cancellationToken);
    }
}
