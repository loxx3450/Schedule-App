﻿using Schedule_App.Core.Models;
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

        Task<T> Add<T>(T obj, CancellationToken cancellationToken)
            where T : class;

        Task Delete<T>(T obj)
            where T : AuditableEntity;

        Task SaveChanges(CancellationToken cancellationToken);
    }
}
