using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly IRepository _repository;
        private readonly IDataHelper _dataHelper;

        public ClassroomService(IRepository repository, IDataHelper dataHelper)
        {
            _repository = repository;
            _dataHelper = dataHelper;
        }

        #region Read
        public async Task<Classroom[]> GetClassrooms(ClassroomFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            if (filter.Title is not null)
            {
                return [await GetClassroomByTitle(filter.Title, cancellationToken)];
            }

            var classroom = _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking();

            // Filters in the future

            return await classroom
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        private async Task<Classroom> GetClassroomByTitle(string title, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Title == title, cancellationToken);

            // Checks if Classroom exists
            EntityValidator.EnsureEntityExists(classroom, nameof(classroom.Title), title);

            return classroom!;
        }


        public async Task<Classroom> GetClassroomById(int id, CancellationToken cancellationToken)
        {
            var classroom = await _dataHelper.GetAuditableEntityByIdAsNoTracking<Classroom>(id, cancellationToken);

            // Checks if Classroom exists
            EntityValidator.EnsureEntityExists(classroom, nameof(classroom.Id), id);

            return classroom!;
        }
        #endregion

        #region Create
        public async Task<Classroom> AddClassroom(Classroom classroom, CancellationToken cancellationToken)
        {
            await EnsureTitleIsNotTaken(classroom.Title, cancellationToken);

            await _repository.AddAuditableEntity(classroom, cancellationToken);
            await _repository.SaveChanges(cancellationToken);

            return classroom;
        }
        #endregion

        #region Delete
        public async Task DeleteClassroom(int id, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            // Checks if Classroom exists
            EntityValidator.EnsureEntityExists(classroom, nameof(classroom.Id), id);

            // Changing state of timestamp's
            await _repository.DeleteSoft(classroom!, cancellationToken);

            // Updating value for Unique Field
            classroom!.Title = $"{classroom!.Title}_deleted_{classroom!.DeletedAt}";

            await _dataHelper.DeleteAssociatedLessons(classroom!.Lessons, cancellationToken);

            await _repository.SaveChanges(cancellationToken);
        }
        #endregion

        #region AdditionalMethods
        private async Task EnsureTitleIsNotTaken(string title, CancellationToken cancellationToken)
        {
            if (await IsTitleTaken(title, cancellationToken))
            {
                throw new ArgumentException($"Classroom with Title '{title}' already exists");
            }
        }

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Classroom>()
                .AnyAsync(c => c.Title == title, cancellationToken);
        }
        #endregion
    }
}
