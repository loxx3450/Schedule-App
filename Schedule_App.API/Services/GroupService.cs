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

        public async Task<IEnumerable<GroupReadSummaryDTO>> GetGroupsSummaries(int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
        {
            var groups = await GetGroups(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadSummaryDTO>>(groups);
        }

        public async Task<IEnumerable<GroupReadFullDTO>> GetGroupsDetailed(int offset, int limit, CancellationToken cancellationToken)
        {
            var groups = await GetGroups(offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadFullDTO>>(groups);
        }

        private async Task<IEnumerable<Group>> GetGroups(int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
        {
            return await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<IEnumerable<GroupReadSummaryDTO>> GetGroupsSummariesByFilter(GroupFilter groupFilter, int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
        {
            var groups = await GetGroupsByFilter(groupFilter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadSummaryDTO>>(groups);
        }

        public async Task<IEnumerable<GroupReadFullDTO>> GetGroupsDetailedByFilter(GroupFilter groupFilter, int offset, int limit, CancellationToken cancellationToken)
        {
            var groups = await GetGroupsByFilter(groupFilter, offset, limit, cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadFullDTO>>(groups);
        }

        private async Task<IEnumerable<Group>> GetGroupsByFilter(GroupFilter groupFilter, int offset = 0, int limit = 20, CancellationToken cancellationToken = default)
        {
            if (groupFilter.Title is not null)
            {
                return [ await GetGroupByTitle(groupFilter.Title, cancellationToken) ];
            }

            if (groupFilter.TeacherId is not null)
            {
                // If teacher does not exist
                if (!await _repository.GetAllNotDeleted<GroupTeacher>()
                    .AnyAsync(gt => gt.TeacherId == groupFilter.TeacherId && gt.Teacher.DeletedAt == null))
                {
                    throw new ArgumentException($"Teacher with ID '{groupFilter.TeacherId}' is not found");
                }

                var groups = _repository.GetAllNotDeleted<GroupTeacher>()
                    .AsNoTracking()
                    .Where(gt => gt.TeacherId == groupFilter.TeacherId && gt.Group.DeletedAt == null)
                    .Select(gt => gt.Group);

                if (groupFilter.TitlePattern is not null)
                {
                    groups = groups.Where(g => g.Title.Contains(groupFilter.TitlePattern));
                }

                return await groups
                    .Skip(offset)
                    .Take(limit)
                    .ToArrayAsync(cancellationToken);
            }

            if (groupFilter.TitlePattern is not null)
            {
                return await GetGroupsByTitlePattern(groupFilter.TitlePattern, offset, limit, cancellationToken);
            }

            return [];
        }

        public async Task<GroupReadSummaryDTO> GetGroupSummaryById(int id, CancellationToken cancellationToken = default)
        {
            var group = await GetGroupById(id, cancellationToken);

            return _mapper.Map<GroupReadSummaryDTO>(group);
        }

        public async Task<GroupReadFullDTO> GetGroupDetailedById(int id, CancellationToken cancellationToken = default)
        {
            var group = await GetGroupById(id, cancellationToken);

            return _mapper.Map<GroupReadFullDTO>(group);
        }

        private async Task<Group> GetGroupById(int id, CancellationToken cancellationToken)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with ID '{id}' is not found");
            }

            return group;
        }

        private async Task<Group> GetGroupByTitle(string title, CancellationToken cancellationToken)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Title == title, cancellationToken);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with Title '{title}' is not found");
            }

            return group;
        }

        private async Task<IEnumerable<Group>> GetGroupsByTitlePattern(string title, int offset, int limit, CancellationToken cancellationToken)
        {
            return await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .Where(g => g.Title.Contains(title))
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<GroupReadSummaryDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken = default)
        {
            var group = _mapper.Map<Group>(groupCreateDTO);

            if (await IsTitlelimitn(groupCreateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Group with Title '{groupCreateDTO.Title}' already exists");
            }

            await _repository.AddAuditableEntity<Group>(group, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<GroupReadSummaryDTO>(group);
        }

        public async Task<GroupReadSummaryDTO> UpdateGroup(int id, GroupUpdateDTO groupUpdateDTO, CancellationToken cancellationToken)
        {
            var group = await _repository.GetAll<Group>()
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with ID '{id}' is not found");
            }

            if (await IsTitlelimitn(groupUpdateDTO.Title, cancellationToken))
            {
                throw new ArgumentException($"Group with Title '{groupUpdateDTO.Title}' already exists");
            }

            group.Title = groupUpdateDTO.Title;
            group.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<GroupReadSummaryDTO>(group);
        }

        public async Task DeleteGroup(int id, CancellationToken cancellationToken = default)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .Include(g => g.Lessons)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with ID '{id}' is not found");
            }

            // Changing state of timestamp's
            await _repository.DeleteSoft<Group>(group, cancellationToken);

            // Updating value for Unique Field
            group.Title = $"{group.Title}_deleted_{group.DeletedAt}";

            // Soft Delete all records, that are associated with this Group
            await DeleteAssociationsWithTeachers(id, cancellationToken);

            await DeleteAssociatedLessons(group.Lessons, cancellationToken);

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

        private async Task DeleteAssociationsWithTeachers(int groupId, CancellationToken cancellationToken)
        {
            var groupTeacherInfos = await _repository.GetAllNotDeleted<GroupTeacher>()
                .Where(gt => gt.GroupId == groupId)
                .ToArrayAsync();

            foreach (var info in groupTeacherInfos)
            {
                await _repository.DeleteSoft<GroupTeacher>(info, cancellationToken);
            }
        }

        private Task<bool> IsTitlelimitn(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Group>()
                .AnyAsync(g => g.Title == title, cancellationToken);
        }
    }
}
