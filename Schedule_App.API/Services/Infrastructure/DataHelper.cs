using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services.Infrastructure
{
    public class DataHelper : IDataHelper
    {
        private const string ID_PROPERTY_NAME = "Id";

        private readonly IRepository _repository;

        public DataHelper(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Retrieves an auditable entity by ID
        /// </summary>
        /// <typeparam name="T">The type of auditable entity</typeparam>
        /// <param name="id">The Id of the entity</param>
        /// <returns>The entity if found, null otherwise</returns>
        public Task<T?> GetAuditableEntityById<T>(int id, CancellationToken cancellationToken)
            where T : AuditableEntity
        {
            return _repository.GetAllNotDeleted<T>()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, ID_PROPERTY_NAME) == id, cancellationToken);
        }

        /// <summary>
        /// Retrieves an entity by ID
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="id">The Id of the entity</param>
        /// <returns>The entity if found, null otherwise</returns>
        public Task<T?> GetEntityById<T>(int id, CancellationToken cancellationToken)
            where T : class
        {
            return _repository.GetAll<T>()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, ID_PROPERTY_NAME) == id, cancellationToken);
        }

        /// <summary>
        /// Retrieves an auditable entity by ID without EF-Tracking
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="id">The Id of the entity</param>
        /// <returns>The entity without EF-Tracking if found, null otherwise</returns>
        public Task<T?> GetAuditableEntityByIdAsNoTracking<T>(int id, CancellationToken cancellationToken)
            where T : AuditableEntity
        {
            return _repository.GetAllNotDeleted<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, ID_PROPERTY_NAME) == id, cancellationToken);
        }

        /// <summary>
        /// Retrieves an entity by ID without EF-Tracking
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="id">The Id of the entity</param>
        /// <returns>The entity without EF-Tracking if found, null otherwise</returns>
        public Task<T?> GetEntityByIdAsNoTracking<T>(int id, CancellationToken cancellationToken)
            where T : class
        {
            return _repository.GetAll<T>()
                .AsNoTracking()
                .FirstOrDefaultAsync(e => EF.Property<int>(e, ID_PROPERTY_NAME) == id, cancellationToken);
        }

        /// <summary>
        /// Ensures that an auditable entity exists by ID
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="id">The Id of the entity</param>
        /// <returns>A Task</returns>
        public async Task EnsureAuditableEntityExistsById<T>(int id, CancellationToken cancellationToken) 
            where T : AuditableEntity
        {
            var entity = await GetAuditableEntityByIdAsNoTracking<T>(id, cancellationToken);

            EntityValidator.EnsureEntityExists(entity, ID_PROPERTY_NAME, id);
        }

        /// <summary>
        /// Ensures that an entity exists by ID
        /// </summary>
        /// <typeparam name="T">The type of entity</typeparam>
        /// <param name="id">The Id of the entity</param>
        /// <returns>A Task</returns>
        public async Task EnsureEntityExistsById<T>(int id, CancellationToken cancellationToken) 
            where T : class
        {
            var entity = await GetEntityByIdAsNoTracking<T>(id, cancellationToken);

            EntityValidator.EnsureEntityExists(entity, ID_PROPERTY_NAME, id);
        }

        /// <summary>
        /// Deletes soft income list of lessons
        /// </summary>
        /// <param name="lessons">The list of lessons</param>
        /// <returns>A Task</returns>
        public async Task DeleteAssociatedLessons(List<Lesson> lessons, CancellationToken cancellationToken)
        {
            foreach (var lesson in lessons)
            {
                if (lesson.DeletedAt is null)
                {
                    await _repository.DeleteSoft(lesson, cancellationToken);
                }
            }
        }
    }
}
