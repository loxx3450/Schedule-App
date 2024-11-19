using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;
using System.Text.RegularExpressions;

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

        public async Task<IEnumerable<TeacherReadDTO>> GetTeachers(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var teachers = await _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadDTO>>(teachers);
        }

        public async Task<IEnumerable<TeacherReadDTO>> GetTeachersByFilter(TeacherFilter filter, int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            if (filter.Username is not null)
            {
                return [ await GetTeacherByUsername(filter.Username, cancellationToken) ];
            }

            if (filter.GroupId is not null)
            {
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
                }

                var result = await teachers.Skip(skip)
                    .Take(take)
                    .ToArrayAsync(cancellationToken);

                return _mapper.Map<IEnumerable<TeacherReadDTO>>(result);
            }

            if (filter.SubjectId is not null)
            {
                return await GetTeachersBySubjectId(filter.SubjectId.Value!, skip, take, cancellationToken);
            }

            return [];
        }

        private async Task<IEnumerable<TeacherReadDTO>> GetTeachersBySubjectId(int subjectId, int skip, int take, CancellationToken cancellationToken)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .Include(s => s.Teachers)
                .FirstOrDefaultAsync(s => s.Id == subjectId, cancellationToken);
                
            if (subject is null)
            {
                return [];
            }

            var teachers = subject.Teachers
                .Skip(skip)
                .Take(take)
                .ToArray();

            return _mapper.Map<IEnumerable<TeacherReadDTO>>(teachers);
        }

        public async Task<TeacherReadDTO> GetTeacherById(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await FindTeacherById(id, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher with ID '{id}' is not found");
            }

            return _mapper.Map<TeacherReadDTO>(teacher);
        }

        private async Task<TeacherReadDTO> GetTeacherByUsername(string username, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Username == username, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher with Username '{username}' is not found");
            }

            return _mapper.Map<TeacherReadDTO>(teacher);
        }

        // TODO: add hashing
        public async Task<TeacherReadDTO> AddTeacher(TeacherCreateDTO teacherCreateDTO, CancellationToken cancellationToken = default)
        {
            // if username is already taken
            if (await IsUsernameTaken(teacherCreateDTO.Username, cancellationToken))
            {
                throw new ArgumentException($"Teacher with Username '{teacherCreateDTO.Username}' already exists");
            }

            var teacher = _mapper.Map<Teacher>(teacherCreateDTO);

            await _repository.AddAuditableEntity<Teacher>(teacher, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<TeacherReadDTO>(teacher);
        }

        public async Task<TeacherReadDTO> UpdateTeacher(int id, TeacherUpdateDTO teacherUpdateDTO, CancellationToken cancellationToken = default)
        {
            var teacher = await FindTeacherById(id, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher with ID '{id}' is not found");
            }

            // if new username is not null and not already taken
            if (! string.IsNullOrEmpty(teacherUpdateDTO.Username))
            {
                if (await IsUsernameTaken(teacherUpdateDTO.Username, cancellationToken))
                {
                    throw new ArgumentException($"Teacher with Username '{teacherUpdateDTO.Username}' already exists");
                }

                teacher.Username = teacherUpdateDTO.Username;
            }
            // TODO: add hashing
            teacher.Password = teacherUpdateDTO.Password ?? teacher.Password;
            teacher.FirstName = teacherUpdateDTO.FirstName ?? teacher.FirstName;
            teacher.LastName = teacherUpdateDTO.LastName ?? teacher.LastName;
            teacher.Age = teacherUpdateDTO.Age ?? teacher.Age;

            teacher.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<TeacherReadDTO>(teacher);
        }

        public async Task DeleteTeacher(int id, CancellationToken cancellationToken = default)
        {
            var teacher = await FindTeacherById(id, cancellationToken);

            if (teacher is null) 
            { 
                throw new KeyNotFoundException($"Teacher with ID '{id}' is not found"); 
            }

            await _repository.Delete(teacher);
            await _repository.SaveChanges(cancellationToken);
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
