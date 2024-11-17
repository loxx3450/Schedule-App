using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;
using Schedule_App.Storage;

namespace Schedule_App.API.Services
{
    public class TeacherSubjectService : ITeacherSubjectService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public TeacherSubjectService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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

            if (!teacher.Subjects.Any(s => s.Id == subjectId))
            {
                throw new ArgumentException($"Subject with ID '{subjectId}' is not associated with this teacher");
            }

            teacher.UpdatedAt = DateTime.UtcNow;

            teacher.Subjects.Remove(subject);

            await _repository.SaveChanges(cancellationToken);
        }

        private async Task<(Teacher teacher, Subject subject)> GetTeacherAndSubject(int teacherId, int subjectId, CancellationToken cancellationToken = default)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .FirstOrDefaultAsync(x => x.Id == teacherId, cancellationToken);

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
