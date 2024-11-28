using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Schedule_App.API.Services.Infrastructure;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Interfaces.Services;
using Schedule_App.Core.Models;
using System.Threading;

namespace Schedule_App.API.Services
{
    public class GroupTeacherService : IGroupTeacherService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IDataHelper _dataHelper;

        public GroupTeacherService(IRepository repository, IMapper mapper, IDataHelper dataHelper)
        {
            _repository = repository;
            _mapper = mapper;
            _dataHelper = dataHelper;
        }

        #region Read
        public async Task<IEnumerable<GroupTeacherReadDTO>> GetGroupTeacherAssociations(int offset, int limit, CancellationToken cancellationToken)
        {
            var result = await GetActualGroupTeacherAssociations()
                .AsNoTracking()
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupTeacherReadDTO>>(result);
        }


        public async Task<GroupTeacherReadDTO> GetGroupTeacherAssociation(int groupId, int teacherId, CancellationToken cancellationToken)
        {
            var result = await GetActualGroupTeacherAssociations()
                .AsNoTracking()
                .FirstOrDefaultAsync(gt => gt.GroupId == groupId && gt.TeacherId == teacherId, cancellationToken);

            return _mapper.Map<GroupTeacherReadDTO>(result);
        }


        public async Task<IEnumerable<GroupTeacherReadDTO>> GetGroupsByTeacherId(int teacherId, int offset, int limit, CancellationToken cancellationToken)
        {
            var result = await GetActualGroupTeacherAssociations()
                .AsNoTracking()
                .Where(gt => gt.TeacherId == teacherId)
                .Select(gt => new GroupTeacherReadDTO()
                {
                    Group = _mapper.Map<GroupReadSummaryDTO>(gt.Group),
                    CreatedAt = gt.CreatedAt,
                    UpdatedAt = gt.UpdatedAt,
                })
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupTeacherReadDTO>>(result);
        }


        public async Task<IEnumerable<GroupTeacherReadDTO>> GetTeachersByGroupId(int groupId, int offset, int limit, CancellationToken cancellationToken)
        {
            var result = await GetActualGroupTeacherAssociations()
                .AsNoTracking()
                .Where(gt => gt.GroupId == groupId)
                .Select(gt => new GroupTeacherReadDTO()
                {
                    Teacher = _mapper.Map<TeacherReadSummaryDTO>(gt.Teacher),
                    CreatedAt = gt.CreatedAt,
                    UpdatedAt = gt.UpdatedAt,
                })
                .Skip(offset)
                .Take(limit)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupTeacherReadDTO>>(result);
        }
        #endregion

        #region Create
        public async Task AddTeacherToGroup(GroupTeacherCreateDTO createDTO, CancellationToken cancellationToken)
        {
            await ValidateCreateDTO(createDTO, cancellationToken);

            var groupTeacher = new GroupTeacher()
            {
                GroupId = createDTO.GroupId,
                TeacherId = createDTO.TeacherId,
            };

            await _repository.AddAuditableEntity(groupTeacher, cancellationToken);

            await UpdateTimestampsByTeacherAndGroup(createDTO.TeacherId, createDTO.GroupId, cancellationToken);

            await _repository.SaveChanges(cancellationToken);
        }

        private async Task ValidateCreateDTO(GroupTeacherCreateDTO createDTO, CancellationToken cancellationToken)
        {
            var groupId = createDTO.GroupId;
            var teacherId = createDTO.TeacherId;

            // Checks if this (not deleted) association does not exist yet
            var alreadyExists = await _repository.GetAllNotDeleted<GroupTeacher>()
                .AnyAsync(gt => gt.GroupId == groupId && gt.TeacherId == teacherId, cancellationToken);

            if (alreadyExists)
            {
                throw new ArgumentException($"GroupTeacher with IDs [{groupId}; {teacherId}] already exists");
            }

            // Checks if Group with groupId exists
            await _dataHelper.EnsureAuditableEntityExistsById<Group>(groupId, cancellationToken);

            // Checks if Teacher with teacherId exists
            await _dataHelper.EnsureAuditableEntityExistsById<Teacher>(teacherId, cancellationToken);
        }

        private async Task UpdateTimestampsByTeacherAndGroup(int teacherId, int groupId, CancellationToken cancellationToken)
        {
            var teacher = await _dataHelper.GetAuditableEntityById<Teacher>(teacherId, cancellationToken);
            var group = await _dataHelper.GetAuditableEntityById<Group>(groupId, cancellationToken);

            // They can not be null, because DTO was already validated
            teacher!.UpdatedAt = DateTime.UtcNow;
            group!.UpdatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Delete
        public async Task RemoveTeacherFromGroup(int groupId, int teacherId, CancellationToken cancellationToken)
        {
            var groupTeacher = await _repository.GetAllNotDeleted<GroupTeacher>()
                .Include(gt => gt.Group)
                .Include(gt => gt.Teacher)
                .FirstOrDefaultAsync(gt => gt.GroupId == groupId && gt.TeacherId == teacherId, cancellationToken);

            // Checks if TeacherGroup exists
            EntityValidator.EnsureEntityExists(
                entity: groupTeacher,
                propertyNames: [nameof(groupTeacher.GroupId), nameof(groupTeacher.TeacherId)],
                propertyValues: [groupId, teacherId]);

            // Changing state of timestamp's
            await _repository.DeleteSoft(groupTeacher!, cancellationToken);

            groupTeacher!.Group.UpdatedAt = DateTime.UtcNow;
            groupTeacher!.Teacher.UpdatedAt = DateTime.UtcNow;

            await _repository.SaveChanges(cancellationToken);
        }
        #endregion

        #region AdditionalMethods
        private IQueryable<GroupTeacher> GetActualGroupTeacherAssociations()
        {
            return _repository.GetAll<GroupTeacher>()
                .Include(gt => gt.Group)
                .Include(gt => gt.Teacher)
                .Where(gt => gt.Group.DeletedAt == null && gt.Teacher.DeletedAt == null);        // Check if FKs reference to existing objects
        }
        #endregion
    }
}
