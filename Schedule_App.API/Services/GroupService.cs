﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Schedule_App.Core.DTOs.Group;
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
            var result = await _repository.GetAll<Group>()
                .AsNoTracking()
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadDTO>>(result);
        }

        public async Task<IEnumerable<GroupReadDTO>> GetGroupsByTitle(string title, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetAll<Group>()
                .AsNoTracking()
                .Where(g => g.Title.Contains(title))
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadDTO>>(result);
        }

        public async Task<IEnumerable<GroupReadDTO>> GetGroupsByTeacherId(int id, int skip = 0, int take = 20, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetAll<GroupTeacher>()
                .AsNoTracking()
                .Where(gt => gt.TeacherId == id)
                .Skip(skip)
                .Take(take)
                .Select(gt => gt.Group)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<IEnumerable<GroupReadDTO>>(result);
        }

        public async Task<GroupReadDTO> GetGroupById(int id, CancellationToken cancellationToken = default)
        {
            var group = await _repository.GetAll<Group>()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group is null)
            {
                throw new KeyNotFoundException($"Group with ID {id} is not found");
            }

            return _mapper.Map<GroupReadDTO>(group);
        }

        public async Task<GroupReadDTO> AddGroup(GroupCreateDTO groupCreateDTO, CancellationToken cancellationToken = default)
        {
            var group = _mapper.Map<Group>(groupCreateDTO);

            group.CreatedAt = DateTime.UtcNow;
            group.UpdatedAt = DateTime.UtcNow;

            await _repository.Add<Group>(group, cancellationToken);

            await _repository.SaveChanges(cancellationToken);

            return _mapper.Map<GroupReadDTO>(group);
        }

        public async Task DeleteGroup(int id, CancellationToken cancellationToken = default)
        {
            var group = await _repository.GetAll<Group>()
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

            if (group is not null)
            {
                await _repository.Delete<Group>(group);
            }
        }
    }
}