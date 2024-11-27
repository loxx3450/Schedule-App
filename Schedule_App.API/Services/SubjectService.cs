using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Subject;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public SubjectService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummaries(int skip, int take, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjects(skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadSummaryDTO>>(subjects);
        }

        public async Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetails(int skip, int take, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjects(skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadFullDTO>>(subjects);
        }

        private Task<Subject[]> GetSubjects(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            return _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<SubjectReadSummaryDTO>> GetSubjectsSummariesByFilter(SubjectFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjectsByFilter(filter, skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadSummaryDTO>>(subjects);
        }

        public async Task<IEnumerable<SubjectReadFullDTO>> GetSubjectsDetailsByFilter(SubjectFilter filter, int skip, int take, CancellationToken cancellationToken)
        {
            var subjects = await GetSubjectsByFilter(filter, skip, take, cancellationToken);

            return _mapper.Map<IEnumerable<SubjectReadFullDTO>>(subjects);
        }

        private async Task<Subject[]> GetSubjectsByFilter(SubjectFilter filter, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            if (filter.Title is not null)
            {
                return [ await GetSubjectByTitle(filter.Title, cancellationToken) ];
            }

            if (filter.TeacherId is not null)
            {
                var teacher = await _repository.GetAllNotDeleted<Teacher>()
                    .AsNoTracking()
                    .Include(t => t.Subjects)
                    .FirstOrDefaultAsync(t => t.Id == filter.TeacherId, cancellationToken);

                if (teacher is null)
                {
                    throw new KeyNotFoundException($"Teacher with ID '{filter.TeacherId}' is not found");
                }

                return teacher.Subjects
                    .Where(s => s.DeletedAt == null)
                    .Skip(skip)
                    .Take(take)
                    .ToArray();
            }

            return [];
        }

        public async Task<SubjectReadSummaryDTO> GetSubjectSummaryById(int id, CancellationToken cancellationToken)
        {
            var subject = await GetSubjectById(id, cancellationToken);

            return _mapper.Map<SubjectReadSummaryDTO>(subject);
        }

        public async Task<SubjectReadFullDTO> GetSubjectDetailsById(int id, CancellationToken cancellationToken)
        {
            var subject = await GetSubjectById(id, cancellationToken);

            return _mapper.Map<SubjectReadFullDTO>(subject);
        }

        private async Task<Subject> GetSubjectById(int id, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' is not found");
            }

            return subject;
        }

        private async Task<Subject> GetSubjectByTitle(string title, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Title == title, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with Title '{title}' is not found");
            }

            return subject;
        }

        public async Task<SubjectReadSummaryDTO> AddSubject(SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken = default)
        {
            // if title is already taken
            if (await IsTitleTaken(subjectCreateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Subject with Title '{subjectCreateDTO.Title}' already exists");
            }

            var subject = _mapper.Map<Subject>(subjectCreateDTO);

            await _repository.AddAuditableEntity<Subject>(subject, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<SubjectReadSummaryDTO>(subject);
        }

        public async Task<SubjectReadSummaryDTO> UpdateSubjectTitle(int id, SubjectUpdateDTO subjectUpdateDTO, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' is not found");
            }

            // if title is already taken
            if (await IsTitleTaken(subjectUpdateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Subject with Title '{subjectUpdateDTO.Title}' already exists");
            }

            subject.Title = subjectUpdateDTO.Title;

            subject.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<SubjectReadSummaryDTO>(subject);
        }

        public async Task DeleteSubject(int id, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .Include(s => s.Lessons)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' is not found");
            }

            // Changing state of timestamp's
            await _repository.DeleteSoft<Subject>(subject);

            // Updating value for Unique Field
            subject.Title = $"{subject.Title}_deleted_{subject.DeletedAt}";

            await DeleteAssociatedLessons(subject.Lessons, cancellationToken);

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

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Subject>()
                .AnyAsync(s => s.Title == title);
        }
    }
}
