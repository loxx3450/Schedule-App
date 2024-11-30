using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class GroupService : IGroupService
    {
        private readonly IRepository _repository;
        private readonly IDataHelper _dataHelper;

        public GroupService(IRepository repository, IDataHelper dataHelper)
        {
            _repository = repository;
            _dataHelper = dataHelper;
        }

        #region Read
        public async Task<Group[]> GetGroups(GroupFilter groupFilter, int offset, int limit, CancellationToken cancellationToken)
        {
            if (groupFilter.Title is not null)
            {
                return [await GetGroupByTitle(groupFilter.Title, cancellationToken)];
            }

            var groups = _repository.GetAllNotDeleted<Group>()
                .AsNoTracking();

            // Filtering groups
            if (groupFilter.TeacherId is not null)
            {
                groups = await FilterGroupsByTeacher(groups, groupFilter.TeacherId.Value, cancellationToken);
            }
            if (groupFilter.TitlePattern is not null)
            {
                groups = groups.Where(g => g.Title.Contains(groupFilter.TitlePattern));
            }

            return await groups
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);
        }

        private async Task<IQueryable<Group>> FilterGroupsByTeacher(IQueryable<Group> groups, int teacherId, CancellationToken cancellationToken)
        {
            // Check if Teacher exists
            await _dataHelper.EnsureAuditableEntityExistsById<Teacher>(teacherId, cancellationToken);

            // Selecting array of group's ids that are associated with this Teacher
            var groupsIds = _repository.GetAllNotDeleted<GroupTeacher>()
                .AsNoTracking()
                .Where(gt => gt.TeacherId == teacherId)
                .Select(gt => gt.GroupId);

            return groups.Where(g => groupsIds.Contains(g.Id));
        }

        private async Task<Group> GetGroupByTitle(string title, CancellationToken cancellationToken)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Title == title, cancellationToken);

            // Check if Group exists
            EntityValidator.EnsureEntityExists(group, nameof(group.Title), title);

            return group!;
        }


        public async Task<Group> GetGroupById(int id, CancellationToken cancellationToken)
        {
            var group = await _dataHelper.GetAuditableEntityByIdAsNoTracking<Group>(id, cancellationToken);

            // Check if Group exists
            EntityValidator.EnsureEntityExists(group, nameof(group.Id), id);

            return group!;
        }
        #endregion

        #region Create
        public async Task<Group> AddGroup(Group group, CancellationToken cancellationToken = default)
        {
            await EnsureTitleIsNotTaken(group.Title, cancellationToken);

            await _repository.AddAuditableEntity<Group>(group, cancellationToken);
            await _repository.SaveChanges(cancellationToken);

            return group;
        }
        #endregion

        #region Update
        public async Task<Group> UpdateGroup(int id, GroupUpdateDTO groupUpdateDTO, CancellationToken cancellationToken)
        {
            var group = await _dataHelper.GetAuditableEntityById<Group>(id, cancellationToken);

            // Check if Group exists
            EntityValidator.EnsureEntityExists(group, nameof(group.Id), id);

            await EnsureTitleIsNotTaken(groupUpdateDTO.Title, cancellationToken);

            group!.Title = groupUpdateDTO.Title;
            group!.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);

            return group!;
        }
        #endregion

        #region Delete
        public async Task DeleteGroup(int id, CancellationToken cancellationToken = default)
        {
            var group = await _repository.GetAllNotDeleted<Group>()
                .Include(g => g.Lessons)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            // Check if Group exists
            EntityValidator.EnsureEntityExists(group, nameof(group.Id), id);

            // Changing state of timestamp's
            await _repository.DeleteSoft<Group>(group, cancellationToken);

            // Updating value for Unique Field
            group.Title = $"{group.Title}_deleted_{group.DeletedAt}";

            // Deletes soft all records with Teachers, that are associated with this Group
            await DeleteAssociationsWithTeachers(id, cancellationToken);

            await _dataHelper.DeleteAssociatedLessons(group.Lessons, cancellationToken);

            await _repository.SaveChanges(cancellationToken);
        }

        private async Task DeleteAssociationsWithTeachers(int groupId, CancellationToken cancellationToken)
        {
            var groupTeacherAssociations = await _repository.GetAllNotDeleted<GroupTeacher>()
                .Where(gt => gt.GroupId == groupId)
                .ToArrayAsync(cancellationToken);

            foreach (var association in groupTeacherAssociations)
            {
                await _repository.DeleteSoft(association, cancellationToken);
            }
        }
        #endregion

        #region AdditionalMethods
        private async Task EnsureTitleIsNotTaken(string title, CancellationToken cancellationToken)
        {
            if (await IsTitleTaken(title, cancellationToken))
            {
                throw new ArgumentException($"Group with Title '{title}' already exists");
            }
        }

        private Task<bool> IsTitleTaken(string title, CancellationToken cancellationToken)
        {
            return _repository.GetAll<Group>()
                .AnyAsync(g => g.Title == title, cancellationToken);
        }
        #endregion
    }
}
