using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.DTOs.GroupTeacher;
using Schedule_App.Core.DTOs.Teacher;
using Schedule_App.Core.Interfaces;
using Schedule_App.Core.Models;

namespace Schedule_App.API.Services
{
    public class GroupTeacherService : IGroupTeacherService
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;

        public GroupTeacherService(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<GroupTeacherReadDTO>> GetGroupTeacherInfos(int skip, int take, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllNotDeleted<GroupTeacher>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupTeacherReadDTO>>(result);
        }

        public async Task<GroupTeacherReadDTO> GetGroupTeacherInfo(int groupId, int teacherId, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllNotDeleted<GroupTeacher>()
                .AsNoTracking()
                .FirstOrDefaultAsync(gt => gt.GroupId == groupId && gt.TeacherId == teacherId);

            return _mapper.Map<GroupTeacherReadDTO>(result);
        }

        public async Task<IEnumerable<GroupTeacherReadDTO>> GetGroupsByTeacherId(int teacherId, int skip, int take, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllNotDeleted<GroupTeacher>()
                .AsNoTracking()
                .Where(gt => gt.TeacherId == teacherId)
                .Select(gt => new GroupTeacherReadDTO()
                {
                    Group = _mapper.Map<GroupReadDTO>(gt.Group),
                    CreatedAt = gt.CreatedAt,
                    UpdatedAt = gt.UpdatedAt,
                })
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupTeacherReadDTO>>(result);
        }

        public async Task<IEnumerable<GroupTeacherReadDTO>> GetTeachersByGroupId(int groupId, int skip, int take, CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllNotDeleted<GroupTeacher>()
                .AsNoTracking()
                .Where(gt => gt.GroupId == groupId)
                .Select(gt => new GroupTeacherReadDTO()
                {
                    Teacher = _mapper.Map<TeacherReadDTO>(gt.Teacher),
                    CreatedAt = gt.CreatedAt,
                    UpdatedAt = gt.UpdatedAt,
                })
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupTeacherReadDTO>>(result);
        }

        public async Task AddTeacherToGroup(GroupTeacherCreateDTO createDTO, CancellationToken cancellationToken)
        {
            await ValidateCreateDTO(createDTO, cancellationToken);

            var groupTeacher = new GroupTeacher()
            {
                GroupId = createDTO.GroupId,
                TeacherId = createDTO.TeacherId,
            };

            await _repository.AddAuditableEntity<GroupTeacher>(groupTeacher, cancellationToken);
            await _repository.SaveChanges(cancellationToken);
        }

        private async Task ValidateCreateDTO(GroupTeacherCreateDTO createDTO, CancellationToken cancellationToken)
        {
            var groupId = createDTO.GroupId;
            var teacherId = createDTO.TeacherId;

            // Check if such association does not exist yet
            var alreadyExists = await _repository.GetAllNotDeleted<GroupTeacher>()
                .AnyAsync(gt => gt.GroupId == groupId && gt.TeacherId == teacherId, cancellationToken);

            if (alreadyExists)
            {
                throw new ArgumentException($"GroupTeacher with IDs [{groupId}; {teacherId}] already exists");
            }

            // Check if Group with groupId exists
            var groupIsFound = await _repository.GetAllNotDeleted<Group>()
                .AnyAsync(g => g.Id == groupId);

            if (!groupIsFound)
            {
                throw new ArgumentException($"Group with ID '{groupId}' is not found");
            }

            // Check if Teacher with teacherId exists
            var teacherIsFound = await _repository.GetAllNotDeleted<Teacher>()
                .AnyAsync(t => t.Id == teacherId);

            if (!teacherIsFound)
            {
                throw new ArgumentException($"Teacher with ID '{teacherId}' is not found");
            }
        }

        public async Task RemoveTeacherFromGroup(int groupId, int teacherId, CancellationToken cancellationToken)
        {
            var groupTeacher = await _repository.GetAllNotDeleted<GroupTeacher>()
                .FirstOrDefaultAsync(gt => gt.GroupId == groupId && gt.TeacherId == teacherId, cancellationToken);

            if (groupTeacher is null)
            {
                throw new KeyNotFoundException($"GroupTeacher with IDs [{groupId}; {teacherId}] is not found");
            }

            await _repository.Delete<GroupTeacher>(groupTeacher);
            await _repository.SaveChanges(cancellationToken);
        }
    }
}
