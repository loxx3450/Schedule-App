using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public ClassroomService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClassroomReadDTO>> GetClassrooms(int skip, int take, CancellationToken cancellationToken)
        {
            var classrooms = await _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();

            return _mapper.Map<IEnumerable<ClassroomReadDTO>>(classrooms);
        }

        public async Task<IEnumerable<ClassroomReadDTO>> GetClassroomsByFilter(ClassroomFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
            if (filter.Title is not null)
            {
                return [ await GetClassroomByTitle(filter.Title, cancellationToken) ];
            }

            return [];
        }

        public async Task<ClassroomReadDTO> GetClassroomById(int id, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (classroom is null)
            {
                throw new KeyNotFoundException($"Classroom with ID '{id}' is not found");
            }

            return _mapper.Map<ClassroomReadDTO>(classroom);
        }

        private async Task<ClassroomReadDTO> GetClassroomByTitle(string title, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Title == title, cancellationToken);

            if (classroom is null)
            {
                throw new KeyNotFoundException($"Classroom with Title '{title}' is not found");
            }

            return _mapper.Map<ClassroomReadDTO>(classroom);
        }

        public async Task<ClassroomReadDTO> AddClassroom(ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken)
        {
            if (await IsTitleTaken(classroomCreateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Classroom with Title '{classroomCreateDTO.Title}' already exists");
            }

            var classroom = _mapper.Map<Classroom>(classroomCreateDTO);

            await _repository.AddAuditableEntity<Classroom>(classroom);
            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<ClassroomReadDTO>(classroom);
        }

        public async Task DeleteClassroom(int id, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (classroom is null)
            {
                throw new KeyNotFoundException($"Classroom with ID '{id}' is not found");
            }

            // Changing state of timestamp's
            await _repository.DeleteSoft<Classroom>(classroom);

            // Updating value for Unique Field
            classroom.Title = $"{classroom.Title}_deleted_{classroom.DeletedAt}";

            await _repository.SaveChanges(cancellationToken);
        }

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Classroom>()
                .AnyAsync(c => c.Title == title, cancellationToken);
        }
    }
}
