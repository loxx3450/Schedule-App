using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class LessonStatusService : ILessonStatusService
    {
        private readonly IRepository _repository;

        public LessonStatusService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<LessonStatus>> GetLessonStatuses(CancellationToken cancellationToken)
        {
            return await _repository.GetAll<LessonStatus>()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<LessonStatus> GetLessonStatusById(int id, CancellationToken cancellationToken)
        {
            var lessonStatus = await _repository.GetAll<LessonStatus>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (lessonStatus is null)
            {
                throw new KeyNotFoundException($"Lesson status with ID {id} is not found");
            }

            return lessonStatus;
        }
    }
}
