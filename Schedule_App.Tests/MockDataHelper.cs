using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;
using Schedule_App.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Tests
{
    class MockDataHelper : IDataHelper
    {
        private const string ID_PROPERTY_NAME = "Id";

        private readonly IRepository _repository;

        public MockDataHelper(IRepository repository)
        {
            _repository = repository;
        }

        public Task DeleteAssociatedLessons(List<Lesson> lessons, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task EnsureAuditableEntityExistsById<T>(int id, CancellationToken cancellationToken) where T : AuditableEntity
        {
            var entity = await GetAuditableEntityByIdAsNoTracking<T>(id, cancellationToken);

            EntityValidator.EnsureEntityExists(entity, ID_PROPERTY_NAME, id);
        }

        public async Task EnsureEntityExistsById<T>(int id, CancellationToken cancellationToken) where T : class
        {
            var entity = await GetEntityByIdAsNoTracking<T>(id, cancellationToken);

            EntityValidator.EnsureEntityExists(entity, ID_PROPERTY_NAME, id);
        }

        public Task<T?> GetAuditableEntityById<T>(int id, CancellationToken cancellationToken) where T : AuditableEntity
        {
            var property = typeof(T).GetProperty(ID_PROPERTY_NAME);

            return _repository.GetAll<T>()
                .FirstOrDefaultAsync((T e) => (int)property.GetValue(e) == id && e.DeletedAt == null, cancellationToken);
        }

        public Task<T?> GetAuditableEntityByIdAsNoTracking<T>(int id, CancellationToken cancellationToken) where T : AuditableEntity
        {
            var property = typeof(T).GetProperty(ID_PROPERTY_NAME);

            return _repository.GetAll<T>()
                .FirstOrDefaultAsync((T e) => (int)property.GetValue(e) == id && e.DeletedAt == null, cancellationToken);
        }

        public Task<T?> GetEntityById<T>(int id, CancellationToken cancellationToken) where T : class
        {
            var property = typeof(T).GetProperty(ID_PROPERTY_NAME);

            return _repository.GetAll<T>()
                .FirstOrDefaultAsync((T e) => (int)property.GetValue(e) == id, cancellationToken);
        }

        public Task<T?> GetEntityByIdAsNoTracking<T>(int id, CancellationToken cancellationToken) where T : class
        {
            var property = typeof(T).GetProperty(ID_PROPERTY_NAME);

            return _repository.GetAll<T>()
                .FirstOrDefaultAsync((T e) => (int)property.GetValue(e) == id, cancellationToken);
        }
    }
}
