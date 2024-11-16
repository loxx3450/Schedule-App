using Schedule_App.Core.Interfaces;
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

        public IQueryable<T> GetAll<T>() where T : class
        {
            return _context.Set<T>();
        }

        public async Task<T> Add<T>(T obj) where T : class
        {
            await _context.Set<T>()
                .AddAsync(obj);

            await _context.SaveChangesAsync();

            return obj;
        }

        public Task Delete<T>(T obj) where T : class
        {
            _context.Set<T>().Remove(obj);

            return Task.CompletedTask;
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
