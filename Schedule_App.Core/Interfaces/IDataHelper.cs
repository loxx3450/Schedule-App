using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface IDataHelper
    {
        // === Get ===
        public Task<T?> GetAuditableEntityById<T>(int id, CancellationToken cancellationToken)
            where T : AuditableEntity;

        public Task<T?> GetEntityById<T>(int id, CancellationToken cancellationToken)
            where T : class;


        // === GetAsNoTracking ===
        public Task<T?> GetAuditableEntityByIdAsNoTracking<T>(int id, CancellationToken cancellationToken)
            where T : AuditableEntity;

        public Task<T?> GetEntityByIdAsNoTracking<T>(int id, CancellationToken cancellationToken)
            where T : class;


        // === EnsureExists ===
        public Task EnsureAuditableEntityExistsById<T>(int id, CancellationToken cancellationToken)
            where T : AuditableEntity;

        public Task EnsureEntityExistsById<T>(int id, CancellationToken cancellationToken)
            where T : class;


        // === AdditionalMethods ===
        public Task DeleteAssociatedLessons(List<Lesson> lessons, CancellationToken cancellationToken);
    }
}
