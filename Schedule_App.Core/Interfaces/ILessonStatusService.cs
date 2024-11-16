using Schedule_App.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Core.Interfaces
{
    public interface ILessonStatusService
    {
        Task<IEnumerable<LessonStatus>> GetLessonStatuses(CancellationToken cancellationToken);

        Task<LessonStatus> GetLessonStatusById(int id, CancellationToken cancellationToken);
    }
}
