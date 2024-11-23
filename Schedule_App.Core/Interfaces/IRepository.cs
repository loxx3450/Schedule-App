using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface IRepository
    {
        IQueryable<T> GetAll<T>()
            where T : class;

        IQueryable<T> GetAllNotDeleted<T>()
            where T : AuditableEntity;

        Task<T> AddAuditableEntity<T>(T obj, CancellationToken cancellationToken = default)
            where T : AuditableEntity;

        Task DeleteSoft<T>(T obj, CancellationToken cancellationToken = default)
            where T : AuditableEntity;

        Task SaveChanges(CancellationToken cancellationToken);
    }
}
