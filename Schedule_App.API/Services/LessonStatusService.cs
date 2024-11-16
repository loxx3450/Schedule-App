using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class LessonStatusService : ILessonStatusService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public LessonStatusService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LessonStatusReadDTO>> GetLessonStatuses(CancellationToken cancellationToken = default)
        {
            var lessonStatuses = await _repository.GetAll<LessonStatus>()
                .AsNoTracking()
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<LessonStatusReadDTO>>(lessonStatuses);
        }

        public async Task<LessonStatusReadDTO> GetLessonStatusById(int id, CancellationToken cancellationToken = default)
        {
            var lessonStatus = await _repository.GetAll<LessonStatus>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (lessonStatus is null)
            {
                throw new KeyNotFoundException($"Lesson status with ID {id} is not found");
            }

            return _mapper.Map<LessonStatusReadDTO>(lessonStatus);
        }
    }
}
