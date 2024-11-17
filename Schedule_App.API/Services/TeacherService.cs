﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.Teacher;
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

        public async Task<IEnumerable<TeacherReadDTO>> GetTeachers(int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var teachers = await _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadDTO>>(teachers);
        }

        public async Task<IEnumerable<TeacherReadDTO>> GetTeachersByGroupId(int groupId, int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var teachers = await _repository.GetAllNotDeleted<GroupTeacher>()
                .AsNoTracking()
                .Where(gt => gt.GroupId == groupId)
                .Skip(skip)
                .Take(take)
                .Select(gt => gt.Teacher)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<TeacherReadDTO>>(teachers);
        }

        public async Task<IEnumerable<TeacherReadDTO>> GetTeachersBySubjectId(int subjectId, int skip = 0, int take = 10, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .Include(s => s.Teachers)
                .FirstOrDefaultAsync(s => s.Id == subjectId, cancellationToken);
                
            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{subjectId}' is not found");
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

        public async Task<TeacherReadDTO> GetTeacherByUsername(string username, CancellationToken cancellationToken = default)
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

        public async Task AddSubjectToTeacher(int teacherId, int subjectId, CancellationToken cancellationToken = default)
        {
            var (teacher, subject) = await GetTeacherAndSubject(teacherId, subjectId, cancellationToken);

            if (teacher.Subjects.Any(s => s.Id == subjectId))
            {
                throw new ArgumentException($"Subject with ID '{subjectId}' is already associated with this teacher");
            }

            teacher.UpdatedAt = DateTime.UtcNow;

            teacher.Subjects.Add(subject);

            await _repository.SaveChanges(cancellationToken);
        }

        public async Task RemoveSubjectFromTeacher(int teacherId, int subjectId, CancellationToken cancellationToken = default)
        {
            var (teacher, subject) = await GetTeacherAndSubject(teacherId, subjectId, cancellationToken);

            if (! teacher.Subjects.Any(s => s.Id == subjectId))
            {
                throw new ArgumentException($"Subject with ID '{subjectId}' is not associated with this teacher");
            }

            teacher.UpdatedAt = DateTime.UtcNow;

            teacher.Subjects.Remove(subject);

            await _repository.SaveChanges(cancellationToken);
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

        private async Task<(Teacher teacher, Subject subject)> GetTeacherAndSubject(int teacherId, int subjectId, CancellationToken cancellationToken = default)
        {
            var teacher = await FindTeacherById(teacherId, cancellationToken);

            if (teacher is null)
            {
                throw new KeyNotFoundException($"Teacher with ID '{teacherId}' is not found");
            }

            var subject = await _repository.GetAllNotDeleted<Subject>()
                .FirstOrDefaultAsync(s => s.Id == subjectId, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{subjectId}' is not found");
            }

            return (teacher, subject);
        }
    }
}