using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class LessonStatusService : ILessonStatusService
    {
        private readonly IRepository _repository;
        private readonly IDataHelper _dataHelper;

        public LessonStatusService(IRepository repository, IDataHelper dataHelper)
        {
            _repository = repository;
            _dataHelper = dataHelper;
        }

        public Task<LessonStatus[]> GetLessonStatuses(CancellationToken cancellationToken)
        {
            return _repository.GetAll<LessonStatus>()
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);
        }

        public async Task<LessonStatus> GetLessonStatusById(int id, CancellationToken cancellationToken)
        {
            var lessonStatus = await _dataHelper.GetEntityByIdAsNoTracking<LessonStatus>(id, cancellationToken);

            // Checks if LessonStatus exists
            EntityValidator.EnsureEntityExists(lessonStatus, nameof(lessonStatus.Id), id);

            return lessonStatus!;
        }
    }
}
