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
        private readonly IMapper _mapper;
        private readonly IDataHelper _dataHelper;

        public LessonStatusService(IRepository repository, IMapper mapper, IDataHelper dataHelper)
        {
            _repository = repository;
            _mapper = mapper;
            _dataHelper = dataHelper;
        }

        public async Task<IEnumerable<LessonStatusReadDTO>> GetLessonStatuses(CancellationToken cancellationToken)
        {
            var lessonStatuses = await _repository.GetAll<LessonStatus>()
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<LessonStatusReadDTO>>(lessonStatuses);
        }

        public async Task<LessonStatusReadDTO> GetLessonStatusById(int id, CancellationToken cancellationToken)
        {
            var lessonStatus = await _dataHelper.GetEntityByIdAsNoTracking<LessonStatus>(id, cancellationToken);

            // Checks if LessonStatus exists
            EntityValidator.EnsureEntityExists(lessonStatus, nameof(lessonStatus.Id), id);

            return _mapper.Map<LessonStatusReadDTO>(lessonStatus);
        }
    }
}
