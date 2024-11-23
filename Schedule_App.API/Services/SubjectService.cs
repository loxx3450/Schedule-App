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

        public async Task<IEnumerable<SubjectReadDTO>> GetSubjects(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync();

            return _mapper.Map<IEnumerable<SubjectReadDTO>>(result);
        }

        public async Task<IEnumerable<SubjectReadDTO>> GetSubjectsByFilter(SubjectFilter filter, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
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

                var subjects = teacher.Subjects
                    .Where(s => s.DeletedAt == null)
                    .Skip(skip)
                    .Take(take)
                    .ToArray();

                return _mapper.Map<IEnumerable<SubjectReadDTO>>(subjects);
            }

            return [];
        }

        public async Task<SubjectReadDTO> GetSubjectById(int id, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' is not found");
            }

            return _mapper.Map<SubjectReadDTO>(subject);
        }

        private async Task<SubjectReadDTO> GetSubjectByTitle(string title, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Title == title, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with Title '{title}' is not found");
            }

            return _mapper.Map<SubjectReadDTO>(subject);
        }

        public async Task<SubjectReadDTO> AddSubject(SubjectCreateDTO subjectCreateDTO, CancellationToken cancellationToken = default)
        {
            // if title is already taken
            if (await IsTitleTaken(subjectCreateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Subject with Title '{subjectCreateDTO.Title}' already exists");
            }

            var subject = _mapper.Map<Subject>(subjectCreateDTO);

            await _repository.AddAuditableEntity<Subject>(subject, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<SubjectReadDTO>(subject);
        }

        public async Task<SubjectReadDTO> UpdateSubjectTitle(int id, string newTitle, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' is not found");
            }

            // if title is already taken
            if (await IsTitleTaken(newTitle, cancellationToken))
            {
                throw new ArgumentException($"Subject with Title '{newTitle}' already exists");
            }

            subject.Title = newTitle;

            subject.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<SubjectReadDTO>(subject);
        }

        public async Task DeleteSubject(int id, CancellationToken cancellationToken = default)
        {
            var subject = await _repository.GetAllNotDeleted<Subject>()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

            if (subject is null)
            {
                throw new KeyNotFoundException($"Subject with ID '{id}' is not found");
            }

            // Changing state of timestamp's
            await _repository.DeleteSoft<Subject>(subject);

            // Updating value for Unique Field
            subject.Title = $"{subject.Title}_deleted_{subject.DeletedAt}";

            await _repository.SaveChanges(cancellationToken);
        }

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Subject>()
                .AnyAsync(s => s.Title == title);
        }
    }
}
