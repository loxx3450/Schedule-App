﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDataHelper _dataHelper;

        public SubjectService(IRepository repository, IMapper mapper, IDataHelper dataHelper)
        {
            _repository = repository;
            _mapper = mapper;
            _dataHelper = dataHelper;
        }

        #region Read
        public async Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummaries(int offset, int limit, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjects(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadSummaryDTO>>(subjects);
        }

        public async Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetailed(int offset, int limit, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjects(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadFullDTO>>(subjects);
        }

        private Task<Subject[]> GetSubjects(int offset, int limit, CancellationToken cancellationToken)
        {
            return _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }



        public async Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummariesByFilter(SubjectFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjectsByFilter(filter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadSummaryDTO>>(subjects);
        }

        public async Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetailedByFilter(SubjectFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjectsByFilter(filter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadFullDTO>>(subjects);
        }

        private async Task<Subject[]> GetSubjectsByFilter(SubjectFilter filter, int offset, int limit, CancellationToken cancellationToken)
        {
            if (filter.Title is not null)
            {
                return [await GetSubjectByTitle(filter.Title, cancellationToken)];
            }

            var subjects = _repository.GetAllNotDeleted<Subject>()
                    .AsNoTracking();

            // Filtering subjects
            if (filter.TeacherId is not null)
            {
                subjects = await FilterSubjectsByTeacher(subjects, filter.TeacherId.Value, cancellationToken);
            }

            return await subjects
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        private async Task<IQueryable<Subject>> FilterSubjectsByTeacher(IQueryable<Subject> subjects, int teacherId, CancellationToken cancellationToken)
        {
            var teacher = await _repository.GetAllNotDeleted<Teacher>()
                .AsNoTracking()
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(t => t.Id == teacherId, cancellationToken);

            // Check if this Teacher exists
            EntityValidator.EnsureEntityExists(teacher, nameof(teacher.Id), teacherId);

            // Selecting array of subject's ids that are associated with this Teacher
            var subjectsIds = teacher!.Subjects.Select(s => s.Id);

            return subjects.Where(s => subjectsIds.Contains(s.Id));
        }

        private async Task<Subject> GetSubjectByTitle(string title, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Title == title, cancellationToken);

            // Checks if Subject exists
            EntityValidator.EnsureEntityExists(subject, nameof(subject.Title), title);

            return subject!;
        }



        public async Task<SubjectReadSummaryDTO> GetSubjectSummaryById(int id, CancellationToken cancellationToken)
        {
            var subject = await GetSubjectById(id, cancellationToken);

            return _mapper.Map<SubjectReadSummaryDTO>(subject);
        }

        public async Task<SubjectReadFullDTO> GetSubjectDetailedById(int id, CancellationToken cancellationToken)
        {
            var subject = await GetSubjectById(id, cancellationToken);

            return _mapper.Map<SubjectReadFullDTO>(subject);
        }

        private async Task<Subject> GetSubjectById(int id, CancellationToken cancellationToken = default)
        {
            var subject = await _dataHelper.GetAuditableEntityByIdAsNoTracking<Subject>(id, cancellationToken);

            // Checks if Subject exists
            EntityValidator.EnsureEntityExists(subject, nameof(subject.Id), id);

            return subject!;
        }
        #endregion

        #region Create
        public async Task<SubjectReadSummaryDTO> AddSubject(SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken = default)
        {
            // Checks if title is already taken
            await EnsureTitleIsNotTaken(subjectCreateDTO.Title, cancellationToken);

            var subject = _mapper.Map<Subject>(subjectCreateDTO);

            await _repository.AddAuditableEntity<Subject>(subject, cancellationToken);
            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<SubjectReadSummaryDTO>(subject);
        }
        #endregion

        #region Update
        public async Task<SubjectReadSummaryDTO> UpdateSubject(int id, SubjectUpdateDTO subjectUpdateDTO, CancellationToken cancellationToken = default)
        {
            var subject = await GetSubjectById(id, cancellationToken);

            // Checks if Title is already taken
            await EnsureTitleIsNotTaken(subjectUpdateDTO.Title, cancellationToken);

            subject.Title = subjectUpdateDTO.Title;

            subject.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<SubjectReadSummaryDTO>(subject);
        }
        #endregion

        #region Delete
        public async Task DeleteSubject(int id, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .Include(s => s.Lessons)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            // Checks if Subject exists
            EntityValidator.EnsureEntityExists(subject, nameof(subject.Id), id);

            // Changing state of timestamp's
            await _repository.DeleteSoft(subject!, cancellationToken);

            // Updating value for Unique Field
            subject!.Title = $"{subject!.Title}_deleted_{subject!.DeletedAt}";

            // Deletes soft associated lessons
            await _dataHelper.DeleteAssociatedLessons(subject!.Lessons, cancellationToken);

            await _repository.SaveChanges(cancellationToken);
        }
        #endregion

        #region AdditionalMethods
        private async Task EnsureTitleIsNotTaken(string title, CancellationToken cancellationToken)
        {
            if (await IsTitleTaken(title, cancellationToken))
            {
                throw new ArgumentException($"Subject with Title '{title}' already exists");
            }
        }

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Subject>()
                .AnyAsync(s => s.Title == title, cancellationToken);
        }
        #endregion
    }
}
