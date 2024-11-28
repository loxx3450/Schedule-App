using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Classroom;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class ClassroomService : IClassroomService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDataHelper _dataHelper;

        public ClassroomService(IRepository repository, IMapper mapper, IDataHelper dataHelper)
        {
            _repository = repository;
            _mapper = mapper;
            _dataHelper = dataHelper;
        }

        #region Read
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
                .ToArrayAsync(cancellationToken);
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
                return [await GetClassroomByTitle(filter.Title, cancellationToken)];
            }

            var classroom = _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking();

            // Filters in the future

            return await classroom
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        private async Task<Classroom> GetClassroomByTitle(string title, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Title == title, cancellationToken);

            // Checks if Classroom exists
            EntityValidator.EnsureEntityExists(classroom, nameof(classroom.Title), title);

            return classroom!;
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
            var classroom = await _dataHelper.GetAuditableEntityByIdAsNoTracking<Classroom>(id, cancellationToken);

            // Checks if Classroom exists
            EntityValidator.EnsureEntityExists(classroom, nameof(classroom.Id), id);

            return classroom!;
        }
        #endregion

        #region Create
        public async Task<ClassroomReadSummaryDTO> AddClassroom(ClassroomCreateDTO classroomCreateDTO, CancellationToken cancellationToken)
        {
            await EnsureTitleIsNotTaken(classroomCreateDTO.Title, cancellationToken);

            var classroom = _mapper.Map<Classroom>(classroomCreateDTO);

            await _repository.AddAuditableEntity(classroom, cancellationToken);
            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<ClassroomReadSummaryDTO>(classroom);
        }
        #endregion

        #region Delete
        public async Task DeleteClassroom(int id, CancellationToken cancellationToken)
        {
            var classroom = await _repository.GetAllNotDeleted<Classroom>()
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

            // Checks if Classroom exists
            EntityValidator.EnsureEntityExists(classroom, nameof(classroom.Id), id);

            // Changing state of timestamp's
            await _repository.DeleteSoft(classroom!, cancellationToken);

            // Updating value for Unique Field
            classroom!.Title = $"{classroom!.Title}_deleted_{classroom!.DeletedAt}";

            await _dataHelper.DeleteAssociatedLessons(classroom!.Lessons, cancellationToken);

            await _repository.SaveChanges(cancellationToken);
        }
        #endregion

        #region AdditionalMethods
        private async Task EnsureTitleIsNotTaken(string title, CancellationToken cancellationToken)
        {
            if (await IsTitleTaken(title, cancellationToken))
            {
                throw new ArgumentException($"Classroom with Title '{title}' already exists");
            }
        }

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Classroom>()
                .AnyAsync(c => c.Title == title, cancellationToken);
        }
        #endregion
    }
}
