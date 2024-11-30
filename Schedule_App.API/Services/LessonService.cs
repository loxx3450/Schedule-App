using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Lesson;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;
using Schedule_App.Storage;

namespace Schedule_App.API.Services
{
    public class LessonService : ILessonService
    {
        private readonly IRepository _repository;
        private readonly IDataHelper _dataHelper;

        public LessonService(IRepository repository, IDataHelper dataHelper)
        {
            _repository = repository;
            _dataHelper = dataHelper;
        }

        #region Read
        public Task<Lesson[]> GetLessons(LessonFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            var lessons = _repository.GetAllNotDeleted<Lesson>()
                .AsNoTracking();

            // Filtering lessons
            lessons = lessons.Where(l =>
                (filter.StartDate == null || DateOnly.FromDateTime(l.StartsAt) == filter.StartDate) &&
                (filter.ClassroomId == null || l.ClassroomId == filter.ClassroomId) &&
                (filter.SubjectId == null || l.SubjectId == filter.SubjectId) &&
                (filter.GroupId == null || l.GroupId == filter.GroupId) &&
                (filter.TeacherId == null || l.TeacherId == filter.TeacherId) &&
                (filter.StatusId == null || l.StatusId == filter.StatusId));

            return lessons.Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<Lesson> GetLessonById(int id, CancellationToken cancellationToken)
        {
            var lesson = await _dataHelper.GetAuditableEntityByIdAsNoTracking<Lesson>(id, cancellationToken);

            // Checks if Lesson exists
            EntityValidator.EnsureEntityExists(lesson, nameof(lesson.Id), id);

            return lesson!;
        }
        #endregion

        #region Create
        public async Task<Lesson> AddLesson(Lesson lesson, CancellationToken cancellationToken)
        {
            await ValidateLesson(lesson, cancellationToken);

            await _repository.AddAuditableEntity(lesson, cancellationToken);
            await _repository.SaveChanges(cancellationToken);

            // To return DTO with filled DTOs in properties
            var result = await GetLessonById(lesson.Id, cancellationToken);

            return result;
        }

        // Checks if income FKs reference to existing entities
        private async Task ValidateLesson(Lesson lesson, CancellationToken cancellationToken)
        {
            await _dataHelper.EnsureAuditableEntityExistsById<Classroom>(lesson.ClassroomId, cancellationToken);

            await _dataHelper.EnsureAuditableEntityExistsById<Subject>(lesson.SubjectId, cancellationToken);

            await _dataHelper.EnsureAuditableEntityExistsById<Group>(lesson.GroupId, cancellationToken);

            await _dataHelper.EnsureAuditableEntityExistsById<Teacher>(lesson.TeacherId, cancellationToken);

            await _dataHelper.EnsureEntityExistsById<LessonStatus>(lesson.StatusId, cancellationToken);
        }
        #endregion

        #region Update
        public async Task<Lesson> UpdateLesson(int id, LessonUpdateDTO lessonUpdateDTO, CancellationToken cancellationToken)
        {
            await ValidateUpdateDTO(lessonUpdateDTO, cancellationToken);

            var lesson = await _dataHelper.GetAuditableEntityById<Lesson>(id, cancellationToken);

            // Checks if Lesson exists
            EntityValidator.EnsureEntityExists(lesson, nameof(lesson.Id), id);

            // Updating Lesson
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

            return lesson;
        }

        // Checks if income FKs (in case they are given) reference to existing entities
        private async Task ValidateUpdateDTO(LessonUpdateDTO lessonUpdateDTO, CancellationToken cancellationToken)
        {
            if (lessonUpdateDTO.ClassroomId is not null)
                await _dataHelper.EnsureAuditableEntityExistsById<Classroom>(lessonUpdateDTO.ClassroomId.Value, cancellationToken);

            if (lessonUpdateDTO.SubjectId is not null)
                await _dataHelper.EnsureAuditableEntityExistsById<Subject>(lessonUpdateDTO.SubjectId.Value, cancellationToken);

            if (lessonUpdateDTO.GroupId is not null)
                await _dataHelper.EnsureAuditableEntityExistsById<Group>(lessonUpdateDTO.GroupId.Value, cancellationToken);

            if (lessonUpdateDTO.TeacherId is not null)
                await _dataHelper.EnsureAuditableEntityExistsById<Teacher>(lessonUpdateDTO.TeacherId.Value, cancellationToken);

            if (lessonUpdateDTO.StatusId is not null)
                await _dataHelper.EnsureEntityExistsById<LessonStatus>(lessonUpdateDTO.StatusId.Value, cancellationToken);
        }
        #endregion

        #region Delete
        public async Task DeleteLesson(int id, CancellationToken cancellationToken)
        {
            var lesson = await _dataHelper.GetAuditableEntityById<Lesson>(id, cancellationToken);

            // Checks if Lesson exists
            EntityValidator.EnsureEntityExists(lesson, nameof(lesson.Id), id);

            // Changing state of timestamp's
            await _repository.DeleteSoft(lesson!, cancellationToken);
            await _repository.SaveChanges(cancellationToken);
        }
        #endregion
    }
}
