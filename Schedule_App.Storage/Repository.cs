﻿using Microsoft.EntityFrameworkCore;
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

        public IQueryable<T> GetAll<T>() where T : class
        {
            return _context.Set<T>();
        }

        public async Task<T> Add<T>(T obj, CancellationToken cancellationToken = default) where T : class
        {
            await _context.Set<T>()
                .AddAsync(obj, cancellationToken);

            return obj;
        }

        // Soft delete
        public Task Delete<T>(T obj) where T : AuditableEntity
        {
            obj.DeletedAt = DateTime.UtcNow;

            return SaveChanges();
        }

        public Task SaveChanges(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
