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

        public async Task<IEnumerable<ClassroomReadSummaryDTO>> GetClassroomsSummaries(int offset, int limit, CancellationToken cancellationToken)
        {
            var classrooms = await GetClassrooms(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<ClassroomReadSummaryDTO>>(classrooms);
        }

        public async Task<IEnumerable<ClassroomReadFullDTO>> GetClassroomsDetailed(int offset, int limit, CancellationToken cancellationToken)
        {
            var classrooms = await GetClassrooms(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<ClassroomReadFullDTO>>(classrooms);
        }

        private Task<Classroom[]> GetClassrooms(int offset, int limit, CancellationToken cancellationToken)
        {
            return _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<ClassroomReadSummaryDTO>> GetClassroomsSummariesByFilter(ClassroomFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            var classrooms = await GetClassroomsByFilter(filter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<ClassroomReadSummaryDTO>>(classrooms);
        }

        public async Task<IEnumerable<ClassroomReadFullDTO>> GetClassroomsDetailedByFilter(ClassroomFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            var classrooms = await GetClassroomsByFilter(filter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<ClassroomReadFullDTO>>(classrooms);
        }

        private async Task<Classroom[]> GetClassroomsByFilter(ClassroomFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            if (filter.Title is not null)
            {
                return [ await GetClassroomByTitle(filter.Title, cancellationToken) ];
            }

            return [];
        }

        public async Task<ClassroomReadSummaryDTO> GetClassroomSummaryById(int id, CancellationToken cancellationToken)
        {
            var classroom = await GetClassroomById(id, cancellationToken);

            return _mapper.Map<ClassroomReadSummaryDTO>(classroom);
        }

        public async Task<ClassroomReadFullDTO> GetClassroomDetailedById(int id, CancellationToken cancellationToken)
        {
            var classroom = await GetClassroomById(id, cancellationToken);

            return _mapper.Map<ClassroomReadFullDTO>(classroom);
        }

        private async Task<Classroom> GetClassroomById(int id, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (classroom is null)
            {
                throw new KeyNotFoundException($"Classroom with ID '{id}' is not found");
            }

            return classroom;
        }

        private async Task<Classroom> GetClassroomByTitle(string title, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Title == title, cancellationToken);

            if (classroom is null)
            {
                throw new KeyNotFoundException($"Classroom with Title '{title}' is not found");
            }

            return classroom;
        }

        public async Task<ClassroomReadSummaryDTO> AddClassroom(ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken)
        {
            if (await IsTitlelimitn(classroomCreateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Classroom with Title '{classroomCreateDTO.Title}' already exists");
            }

            var classroom = _mapper.Map<Classroom>(classroomCreateDTO);

            await _repository.AddAuditableEntity<Classroom>(classroom, cancellationToken);
            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<ClassroomReadSummaryDTO>(classroom);
        }

        public async Task DeleteClassroom(int id, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            if (classroom is null)
            {
                throw new KeyNotFoundException($"Classroom with ID '{id}' is not found");
            }

            // Changing state of timestamp's
            await _repository.DeleteSoft(classroom, cancellationToken);

            // Updating value for Unique Field
            classroom.Title = $"{classroom.Title}_deleted_{classroom.DeletedAt}";

            await DeleteAssociatedLessons(classroom.Lessons, cancellationToken);

            await _repository.SaveChanges(cancellationToken);
        }

        private async Task DeleteAssociatedLessons(List<Lesson> lessons, CancellationToken cancellationToken)
        {
            foreach (var lesson in lessons)
            {
                if (lesson.DeletedAt is null)
                {
                    await _repository.DeleteSoft(lesson, cancellationToken);
                }
            }
        }

        private Task<bool> IsTitlelimitn(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Classroom>()
                .AnyAsync(c => c.Title == title, cancellationToken);
        }
    }
}
