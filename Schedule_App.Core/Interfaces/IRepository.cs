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

        Task<T> Add<T>(T obj)
            where T : class;

        Task Delete<T>(T obj)
            where T : class;

        Task SaveChanges();
    }
}
