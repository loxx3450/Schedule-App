using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class GroupService : IGroupService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public GroupService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GroupReadDTO>> GetGroups(int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadDTO>>(result);
        }

        public async Task<IEnumerable<GroupReadDTO>> GetGroupsByFilter(GroupFilter groupFilter, int skip, int take, CancellationToken cancellationToken)
        {
            if (groupFilter.Title is not null)
            {
                return [ await GetGroupByTitle(groupFilter.Title, cancellationToken) ];
            }

            if (groupFilter.TeacherId is not null)
            {
                var groups = _repository.GetAllNotDeleted<GroupTeacher>()
                    .AsNoTracking()
                    .Where(gt => gt.TeacherId == groupFilter.TeacherId)
                    .Select(gt => gt.Group);

                if (groupFilter.TitlePattern is not null)
                {
                    groups = groups.Where(g => g.Title.Contains(groupFilter.TitlePattern));
                }

                var result = await groups
                    .Skip(skip)
                    .Take(take)
                    .ToArrayAsync(cancellationToken);

                return _mapper.Map<IEnumerable<GroupReadDTO>>(result);
            }

            if (groupFilter.TitlePattern is not null)
            {
                return await GetGroupsByTitlePattern(groupFilter.TitlePattern, skip, take, cancellationToken);
            }

            return [];
        }

        public async Task<GroupReadDTO> GetGroupById(int id, CancellationToken cancellationToken = default)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with ID '{id}' is not found");
            }

            return _mapper.Map<GroupReadDTO>(group);
        }

        private async Task<GroupReadDTO> GetGroupByTitle(string title, CancellationToken cancellationToken)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Title == title, cancellationToken);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with Title '{title}' is not found");
            }

            return _mapper.Map<GroupReadDTO>(group);
        }

        private async Task<IEnumerable<GroupReadDTO>> GetGroupsByTitlePattern(string title, int skip, int take, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .Where(g => g.Title.Contains(title))
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadDTO>>(result);
        }

        public async Task<GroupReadDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken = default)
        {
            var group = _mapper.Map<Group>(groupCreateDTO);

            if (await IsTitleTaken(groupCreateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Group with Title '{groupCreateDTO.Title}' already exists");
            }

            await _repository.AddAuditableEntity<Group>(group, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<GroupReadDTO>(group);
        }

        public async Task<GroupReadDTO> UpdateGroupTitle(int id, string newTitle, CancellationToken cancellationToken)
        {
            var group = await _repository.GetAll<Group>()
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with ID '{id}' is not found");
            }

            if (await IsTitleTaken(newTitle, cancellationToken))
            {
                throw new ArgumentException($"Group with Title '{newTitle}' already exists");
            }

            group.Title = newTitle;
            group.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<GroupReadDTO>(group);
        }

        public async Task DeleteGroup(int id, CancellationToken cancellationToken = default)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with ID '{id}' is not found");
            }

            await _repository.Delete<Group>(group);

            await _repository.SaveChanges(cancellationToken);
        }

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Group>()
                .AnyAsync(g => g.Title == title, cancellationToken);
        }
    }
}
