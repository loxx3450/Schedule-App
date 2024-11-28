using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDataHelper _dataHelper;

        public TeacherService(IRepository repository, IMapper mapper, IDataHelper dataHelper)
        {
            _repository = repository;
            _mapper = mapper;
            _dataHelper = dataHelper;
        }

        #region Read
        public async Task<IEnumerable<TeacherReadSummaryDTO>> GetTeachersSummaries(int offset, int limit, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachers(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadSummaryDTO>>(teachers);
        }

        public async Task<IEnumerable<TeacherReadFullDTO>> GetTeachersDetailed(int offset, int limit, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachers(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadFullDTO>>(teachers);
        }

        private Task<Teacher[]> GetTeachers(int offset, int limit, CancellationToken cancellationToken)
        {
            return _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }
        


        public async Task<IEnumerable<TeacherReadSummaryDTO>> GetTeachersSummariesByFilter(TeacherFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachersByFilter(filter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadSummaryDTO>>(teachers);
        }

        public async Task<IEnumerable<TeacherReadFullDTO>> GetTeachersDetailedByFilter(TeacherFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachersByFilter(filter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadFullDTO>>(teachers);
        }

        private async Task<Teacher[]> GetTeachersByFilter(TeacherFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            if (filter.Username is not null)
            {
                return [await GetTeacherByUsername(filter.Username, cancellationToken)];
            }

            var teachers = _repository.GetAllNotDeleted<Teacher>()
                    .AsNoTracking();

            // Filtering teachers
            if (filter.GroupId is not null)
            {
                teachers = await FilterTeachersByGroup(teachers, filter.GroupId.Value, cancellationToken);
            }
            if (filter.SubjectId is not null)
            {
                teachers = await FilterTeachersBySubject(teachers, filter.SubjectId.Value, cancellationToken);
            }

            return await teachers
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        private async Task<IQueryable<Teacher>> FilterTeachersByGroup(IQueryable<Teacher> teachers, int groupId, CancellationToken cancellationToken)
        {
            // Check if Group exists
            await _dataHelper.EnsureAuditableEntityExistsById<Group>(groupId, cancellationToken);

            // Selecting array of teacher's ids that are associated with this Group
            var teachersIds = _repository.GetAllNotDeleted<GroupTeacher>()
                .AsNoTracking()
                .Where(gt => gt.GroupId == groupId)
                .Select(gt => gt.TeacherId);

            return teachers.Where(t => teachersIds.Contains(t.Id));
        }

        private async Task<IQueryable<Teacher>> FilterTeachersBySubject(IQueryable<Teacher> teachers, int subjectId, CancellationToken cancellationToken)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .Include(s => s.Teachers)
                .FirstOrDefaultAsync(s => s.Id == subjectId, cancellationToken);

            // Check if Subject exists
            EntityValidator.EnsureEntityExists(subject, nameof(subject.Id), subjectId);

            // Selecting array of teacher's ids that are associated with this Subject
            var teachersIds = subject!.Teachers.Select(teacher => teacher.Id);

            return teachers.Where(t => teachersIds.Contains(t.Id));
        }

        private async Task<Teacher> GetTeacherByUsername(string username, CancellationToken cancellationToken)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Username == username, cancellationToken);

            // Check if this Teacher exists
            EntityValidator.EnsureEntityExists(teacher, nameof(teacher.Username), username);

            return teacher!;
        }



        public async Task<TeacherReadSummaryDTO> GetTeacherSummaryById(int id, CancellationToken cancellationToken)
        {
            var teacher = await GetTeacherById(id, cancellationToken);

            return _mapper.Map<TeacherReadSummaryDTO>(teacher);
        }

        public async Task<TeacherReadFullDTO> GetTeacherDetailedById(int id, CancellationToken cancellationToken)
        {
            var teacher = await GetTeacherById(id, cancellationToken);

            return _mapper.Map<TeacherReadFullDTO>(teacher);
        }

        private async Task<Teacher> GetTeacherById(int id, CancellationToken cancellationToken)
        {
            var teacher = await _dataHelper.GetAuditableEntityByIdAsNoTracking<Teacher>(id, cancellationToken);

            // Check if Teacher exists
            EntityValidator.EnsureEntityExists(teacher, nameof(teacher.Id), id);

            return teacher!;
        }
        #endregion

        #region Create
        public async Task<TeacherReadSummaryDTO> AddTeacher(TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken)
        {
            // if username is already taken
            await EnsureUsernameIsNotTaken(teacherCreateDTO.Username, cancellationToken);

            var teacher = _mapper.Map<Teacher>(teacherCreateDTO);

            // Hashing password
            teacher.Password = PasswordHasher.Hash(teacherCreateDTO.Password);

            await _repository.AddAuditableEntity<Teacher>(teacher, cancellationToken);
            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<TeacherReadSummaryDTO>(teacher);
        }
        #endregion

        #region Update
        public async Task<TeacherReadSummaryDTO> UpdateTeacher(int id, TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken)
        {
            var teacher = await GetTeacherById(id, cancellationToken);

            // if new username is not null and not already taken
            if (teacherUpdateDTO.Username is not null)
            {
                await EnsureUsernameIsNotTaken(teacherUpdateDTO.Username, cancellationToken);

                teacher.Username = teacherUpdateDTO.Username;
            }

            // Hashing password
            if (teacherUpdateDTO.Password is not null)
            {
                teacher.Password = PasswordHasher.Hash(teacherUpdateDTO.Password);
            }

            teacher.FirstName = teacherUpdateDTO.FirstName ?? teacher.FirstName;
            teacher.LastName = teacherUpdateDTO.LastName ?? teacher.LastName;
            teacher.Age = teacherUpdateDTO.Age ?? teacher.Age;

            teacher.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<TeacherReadSummaryDTO>(teacher);
        }
        #endregion

        #region Delete
        public async Task DeleteTeacher(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            // Check if Teacher exists
            EntityValidator.EnsureEntityExists(teacher, nameof(teacher.Id), id);

            // Changing state of timestamp's
            await _repository.DeleteSoft(teacher!, cancellationToken);

            // Updating value for Unique Field
            teacher!.Username = $"{teacher!.Username}_deleted_{teacher!.DeletedAt}";

            // Deletes soft all records with Groups, that are associated with this Teacher
            await DeleteAssociationsWithGroups(id, cancellationToken);

            // Deletes soft associated lessons
            await _dataHelper.DeleteAssociatedLessons(teacher!.Lessons, cancellationToken);

            await _repository.SaveChanges(cancellationToken);
        }

        private async Task DeleteAssociationsWithGroups(int teacherId, CancellationToken cancellationToken)
        {
            var groupTeacherAssociations = await _repository.GetAllNotDeleted<GroupTeacher>()
                .Where(gt => gt.TeacherId == teacherId)
                .ToArrayAsync(cancellationToken);

            foreach (var association in groupTeacherAssociations)
            {
                await _repository.DeleteSoft(association, cancellationToken);
            }
        }
        #endregion

        #region AdditionalMethods
        private async Task EnsureUsernameIsNotTaken(string username, CancellationToken cancellationToken)
        {
            if (await IsUsernameTaken(username, cancellationToken))
            {
                throw new ArgumentException($"Teacher with Username '{username}' already exists");
            }
        }

        private Task<bool> IsUsernameTaken(string username, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Teacher>()
                .AsNoTracking()
                .AnyAsync(t => t.Username == username, cancellationToken);
        }
        #endregion
    }
}
