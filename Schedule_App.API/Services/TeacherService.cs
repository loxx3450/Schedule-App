using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Helpers;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public TeacherService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeacherReadSummaryDTO>> GetTeachersSummaries(int skip, int take, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachers(skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadSummaryDTO>>(teachers);
        }

        public async Task<IEnumerable<TeacherReadFullDTO>> GetTeachersDetails(int skip, int take, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachers(skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadFullDTO>>(teachers);
        }

        private Task<Teacher[]> GetTeachers(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            return _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IEnumerable<TeacherReadSummaryDTO>> GetTeachersSummariesByFilter(TeacherFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachersByFilter(filter, skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadSummaryDTO>>(teachers);
        }

        public async Task<IEnumerable<TeacherReadFullDTO>> GetTeachersDetailsByFilter(TeacherFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
            var teachers = await GetTeachersByFilter(filter, skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadFullDTO>>(teachers);
        }

        private async Task<Teacher[]> GetTeachersByFilter(TeacherFilter filter, int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            if (filter.Username is not null)
            {
                return [ await GetTeacherByUsername(filter.Username, cancellationToken) ];
            }

            if (filter.GroupId is not null)
            {
                if (! await _repository.GetAllNotDeleted<Group>().
                    AnyAsync(g => g.Id == filter.GroupId))
                {
                    throw new ArgumentException($"Group with ID '{filter.GroupId}' is not found");
                }

                var teachers = _repository.GetAllNotDeleted<GroupTeacher>()
                    .AsNoTracking()
                    .Where(gt => gt.GroupId == filter.GroupId)
                    .Select(gt => gt.Teacher);

                // From current list of teachers we check if they're associated with SubjectId(if it's given)
                if (filter.SubjectId is not null)
                {
                    var subject = await _repository.GetAllNotDeleted<Subject>()
                        .AsNoTracking()
                        .Include(s => s.Teachers)
                        .FirstOrDefaultAsync(s => s.Id == filter.SubjectId, cancellationToken);

                    if (subject is not null)
                    {
                        var teacherIds = subject.Teachers.Select(teacher => teacher.Id);
                        teachers = teachers.Where(t => teacherIds.Contains(t.Id));
                    }
                    else
                    {
                        throw new ArgumentException($"Subject with ID '{filter.SubjectId}' is not found");
                    }
                }

                return await teachers.Skip(skip)
                    .Take(take)
                    .ToArrayAsync(cancellationToken);
            }

            if (filter.SubjectId is not null)
            {
                return await GetTeachersBySubjectId(filter.SubjectId.Value!, skip, take, cancellationToken);
            }

            return [];
        }

        private async Task<Teacher[]> GetTeachersBySubjectId(int subjectId, int skip, int take, CancellationToken cancellationToken)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .Include(s => s.Teachers)
                .FirstOrDefaultAsync(s => s.Id == subjectId, cancellationToken);
                
            if (subject is null)
            {
                return [];
            }

            return subject.Teachers
                .Where(t => t.DeletedAt == null)
                .Skip(skip)
                .Take(take)
                .ToArray();
        }

        public async Task<TeacherReadSummaryDTO> GetTeacherSummaryById(int id, CancellationToken cancellationToken)
        {
            var teacher = await GetTeacherById(id, cancellationToken);

            return _mapper.Map<TeacherReadSummaryDTO>(teacher);
        }

        public async Task<TeacherReadFullDTO> GetTeacherDetailsById(int id, CancellationToken cancellationToken)
        {
            var teacher = await GetTeacherById(id, cancellationToken);

            return _mapper.Map<TeacherReadFullDTO>(teacher);
        }

        private async Task<Teacher> GetTeacherById(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await FindTeacherById(id, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher with ID '{id}' is not found");
            }

            return teacher;
        }

        private async Task<Teacher> GetTeacherByUsername(string username, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Username == username, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher with Username '{username}' is not found");
            }

            return teacher;
        }

        public async Task<TeacherReadSummaryDTO> AddTeacher(TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken = default)
        {
            // if username is already taken
            if (await IsUsernameTaken(teacherCreateDTO.Username, cancellationToken))
            {
                throw new ArgumentException($"Teacher with Username '{teacherCreateDTO.Username}' already exists");
            }

            var teacher = _mapper.Map<Teacher>(teacherCreateDTO);

            // Hashing password
            teacher.Password = PasswordHasher.Hash(teacherCreateDTO.Password);

            await _repository.AddAuditableEntity<Teacher>(teacher, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<TeacherReadSummaryDTO>(teacher);
        }

        public async Task<TeacherReadSummaryDTO> UpdateTeacher(int id, TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken = default)
        {
            var teacher = await FindTeacherById(id, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher with ID '{id}' is not found");
            }

            // if new username is not null and not already taken
            if (teacherUpdateDTO.Username is not null)
            {
                if (await IsUsernameTaken(teacherUpdateDTO.Username, cancellationToken))
                {
                    throw new ArgumentException($"Teacher with Username '{teacherUpdateDTO.Username}' already exists");
                }

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

        public async Task DeleteTeacher(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .Include(t => t.Lessons)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (teacher is null) 
            { 
                throw new KeyNotFoundException($"Teacher with ID '{id}' is not found"); 
            }

            // Changing state of timestamp's
            await _repository.DeleteSoft<Teacher>(teacher);

            // Updating value for Unique Field
            teacher.Username = $"{teacher.Username}_deleted_{teacher.DeletedAt}";

            // Soft Delete all records, that are associated with this Teacher
            await DeleteAssociationsWithGroups(id);

            await DeleteAssociatedLessons(teacher.Lessons, cancellationToken);

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

        private async Task DeleteAssociationsWithGroups(int teacherId)
        {
            var groupTeacherInfos = await _repository.GetAllNotDeleted<GroupTeacher>()
                .Where(gt => gt.TeacherId == teacherId)
                .ToArrayAsync();

            foreach (var info in groupTeacherInfos)
            {
                await _repository.DeleteSoft<GroupTeacher>(info);
            }
        }


        private Task<Teacher?> FindTeacherById(int id, CancellationToken cancellationToken)
        {
            return _repository.GetAllNotDeleted<Teacher>()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        
        private Task<bool> IsUsernameTaken(string username, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Teacher>()
                .AnyAsync(t => t.Username == username);
        }
    }
}
