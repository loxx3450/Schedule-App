using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;
using Schedule_App.Storage;

namespace Schedule_App.API.Services
{
    public class LessonService : ILessonService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public LessonService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LessonReadSummaryDTO>> GetLessonsSummaries(int skip, int take, CancellationToken cancellationToken)
        {
            var lessons = await GetLessons(skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<LessonReadSummaryDTO>>(lessons);
        }

        public async Task<IEnumerable<LessonReadFullDTO>> GetLessonsDetails(int skip, int take, CancellationToken cancellationToken)
        {
            var lessons = await GetLessons(skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<LessonReadFullDTO>>(lessons);
        }

        private Task<Lesson[]> GetLessons(int skip, int take, CancellationToken cancellationToken)
        {
            return _repository.GetAllNotDeleted<Lesson>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IEnumerable<LessonReadSummaryDTO>> GetLessonsSummariesByFilter(LessonFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
             var lessons = await GetLessonsByFilter(filter, skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<LessonReadSummaryDTO>>(lessons);
        }

        public async Task<IEnumerable<LessonReadFullDTO>> GetLessonsDetailsByFilter(LessonFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
            var lessons = await GetLessonsByFilter(filter, skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<LessonReadFullDTO>>(lessons);
        }

        private Task<Lesson[]> GetLessonsByFilter(LessonFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
            var lessons = _repository.GetAllNotDeleted<Lesson>()
                .AsNoTracking();

            lessons = lessons.Where(l => 
                (filter.Date == null || l.Date == filter.Date) &&
                (filter.ClassroomId == null || l.ClassroomId == filter.ClassroomId) &&
                (filter.SubjectId == null || l.SubjectId == filter.SubjectId) &&
                (filter.GroupId == null || l.GroupId == filter.GroupId) &&
                (filter.TeacherId == null || l.TeacherId == filter.TeacherId) &&
                (filter.StatusId == null || l.StatusId == filter.StatusId));

            return lessons.Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<LessonReadSummaryDTO> GetLessonSummaryById(int id, CancellationToken cancellationToken)
        {
            var lesson = await GetLessonById(id, cancellationToken);

            return _mapper.Map<LessonReadSummaryDTO>(lesson);
        }
        public async Task<LessonReadFullDTO> GetLessonDetailsById(int id, CancellationToken cancellationToken)
        {
            var lesson = await GetLessonById(id, cancellationToken);

            return _mapper.Map<LessonReadFullDTO>(lesson);
        }

        private async Task<Lesson> GetLessonById(int id, CancellationToken cancellationToken)
        {
            var lesson = await _repository.GetAllNotDeleted<Lesson>()
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (lesson is null)
            {
                throw new KeyNotFoundException($"Lesson with ID '{id}' is not found");
            }

            return lesson;
        }
    }
}
