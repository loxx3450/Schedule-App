using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;
using Schedule_App.Storage;

namespace Schedule_App.API.Services
{
    public class TeacherSubjectService : ITeacherSubjectService
    {
        private readonly IRepository _repository;
        private readonly IDataHelper _dataHelper;

        public TeacherSubjectService(IRepository repository, IDataHelper dataHelper)
        {
            _repository = repository;
            _dataHelper = dataHelper;
        }

        public async Task AddSubjectToTeacher(int teacherId, int subjectId, CancellationToken cancellationToken)
        {
            var (teacher, subject) = await GetTeacherAndSubject(teacherId, subjectId, cancellationToken);

            if (teacher.Subjects.Any(s => s.Id == subjectId))
            {
                throw new ArgumentException($"Subject with ID '{subjectId}' is already associated with Teacher with Id '{teacherId}'");
            }

            teacher.UpdatedAt = DateTime.UtcNow;
            subject.UpdatedAt = DateTime.UtcNow;

            // Adding association
            teacher.Subjects.Add(subject);

            await _repository.SaveChanges(cancellationToken);
        }

        public async Task RemoveSubjectFromTeacher(int teacherId, int subjectId, CancellationToken cancellationToken)
        {
            var (teacher, subject) = await GetTeacherAndSubject(teacherId, subjectId, cancellationToken);

            if (! teacher.Subjects.Any(s => s.Id == subjectId))
            {
                throw new ArgumentException($"Subject with Id '{subjectId}' is not associated with Teacher with Id '{teacherId}'");
            }

            teacher.UpdatedAt = DateTime.UtcNow;
            subject.UpdatedAt = DateTime.UtcNow;

            // Removing association
            teacher.Subjects.Remove(subject);

            await _repository.SaveChanges(cancellationToken);
        }

        private async Task<(Teacher teacher, Subject subject)> GetTeacherAndSubject(int teacherId, int subjectId, CancellationToken cancellationToken)
        {
            var teacher = await _dataHelper.GetAuditableEntityById<Teacher>(teacherId, cancellationToken);
            EntityValidator.EnsureEntityExists(teacher, nameof(teacher.Id), teacherId);

            var subject = await _dataHelper.GetAuditableEntityById<Subject>(subjectId, cancellationToken);
            EntityValidator.EnsureEntityExists(subject, nameof(subject.Id), subjectId);

            // Entities can not be null because of EntityValidator
            return (teacher!, subject!);
        }
    }
}
