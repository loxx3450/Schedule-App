using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Storage
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        public Repository(DataContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll<T>() 
            where T : class
        {
            return _context.Set<T>();
        }

        public IQueryable<T> GetAllNotDeleted<T>() 
            where T : AuditableEntity
        {
            return _context.Set<T>()
                .Where(e => e.DeletedAt == null);
        }

        public async Task<T> AddAuditableEntity<T>(T obj, CancellationToken cancellationToken) 
            where T : AuditableEntity
        {
            obj.CreatedAt = DateTime.UtcNow;
            obj.UpdatedAt = DateTime.UtcNow;

            await _context.Set<T>()
                .AddAsync(obj, cancellationToken);

            return obj;
        }

        public Task DeleteSoft<T>(T obj, CancellationToken cancellationToken) 
            where T : AuditableEntity
        {
            obj.DeletedAt = DateTime.UtcNow;
            obj.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }

        public Task SaveChanges(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
