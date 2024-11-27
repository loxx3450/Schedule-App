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
                (filter.Date == null || DateOnly.FromDateTime(l.StartsAt) == filter.Date) &&
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

        public async Task<LessonReadSummaryDTO> AddLesson(LessonCreateDTO lessonCreateDTO, CancellationToken cancellationToken)
        {
            await ValidateCreateDTO(lessonCreateDTO, cancellationToken);

            var lesson = _mapper.Map<Lesson>(lessonCreateDTO);

            await _repository.AddAuditableEntity(lesson, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            // To return DTO with filled DTOs in properties
            var result = await GetLessonById(lesson.Id, cancellationToken);

            return _mapper.Map<LessonReadSummaryDTO>(result);
        }

        // TODO: Refactor ?Some helper?
        private async Task ValidateCreateDTO(LessonCreateDTO lessonCreateDTO, CancellationToken cancellationToken)
        {
            var classroomIsFound = await _repository.GetAllNotDeleted<Classroom>()
                .AnyAsync(c => c.Id == lessonCreateDTO.ClassroomId, cancellationToken);

            if (!classroomIsFound)
            {
                throw new ArgumentException($"Classroom with ID '{lessonCreateDTO.ClassroomId}' is not found");
            }

            var subjectIsFound = await _repository.GetAllNotDeleted<Subject>()
                .AnyAsync(c => c.Id == lessonCreateDTO.SubjectId, cancellationToken);

            if (!subjectIsFound)
            {
                throw new ArgumentException($"Subject with ID '{lessonCreateDTO.SubjectId}' is not found");
            }

            var groupIsFound = await _repository.GetAllNotDeleted<Group>()
                .AnyAsync(c => c.Id == lessonCreateDTO.GroupId, cancellationToken);

            if (!groupIsFound)
            {
                throw new ArgumentException($"Group with ID '{lessonCreateDTO.GroupId}' is not found");
            }

            var teacherIsFound = await _repository.GetAllNotDeleted<Teacher>()
                .AnyAsync(c => c.Id == lessonCreateDTO.TeacherId, cancellationToken);

            if (!teacherIsFound)
            {
                throw new ArgumentException($"Teacher with ID '{lessonCreateDTO.TeacherId}' is not found");
            }

            var statusIsFound = await _repository.GetAll<LessonStatus>()
                .AnyAsync(c => c.Id == lessonCreateDTO.StatusId, cancellationToken);

            if (!statusIsFound)
            {
                throw new ArgumentException($"LessonStatus with ID '{lessonCreateDTO.StatusId}' is not found");
            }
        }

        public async Task<LessonReadSummaryDTO> UpdateLesson(int id, LessonUpdateDTO lessonUpdateDTO, CancellationToken cancellationToken)
        {
            var lesson = await _repository.GetAllNotDeleted<Lesson>()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (lesson is null)
            {
                throw new KeyNotFoundException($"Lesson with ID '{id}' is not found");
            }

            lesson.ClassroomId = lessonUpdateDTO.ClassroomId ?? lesson.ClassroomId;
            lesson.SubjectId = lessonUpdateDTO.SubjectId ?? lesson.SubjectId;
            lesson.GroupId = lessonUpdateDTO.GroupId ?? lesson.GroupId;
            lesson.TeacherId = lessonUpdateDTO.TeacherId ?? lesson.TeacherId;
            lesson.AdditionalInfo = lessonUpdateDTO.AdditionalInfo ?? lesson.AdditionalInfo;
            lesson.StartsAt = lessonUpdateDTO.StartsAt ?? lesson.StartsAt;
            lesson.EndsAt = lessonUpdateDTO.EndsAt ?? lesson.EndsAt;
            lesson.StatusId = lessonUpdateDTO.StatusId ?? lesson.StatusId;

            lesson.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<LessonReadSummaryDTO>(lesson);
        }

        public async Task DeleteLesson(int id, CancellationToken cancellationToken)
        {
            var lesson = await _repository.GetAllNotDeleted<Lesson>()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);

            if (lesson is null)
            {
                throw new KeyNotFoundException($"Lesson with ID '{id}' is not found");
            }

            // Changing state of timestamp's
            await _repository.DeleteSoft<Lesson>(lesson);

            await _repository.SaveChanges(cancellationToken);
        }
    }
}
