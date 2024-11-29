using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Schedule_App.API;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.API.Services;
using Schedule_App.Core.DTOs.Group;
using Schedule_App.Core.Filters;
using Schedule_App.Core.Models;
using Schedule_App.Tests.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Tests.Tests
{
    public class GroupServiceTests : BaseTestsClass
    {
        private const int DEFAULT_OFFSET = 0;
        private const int DEFAULT_LIMIT = 20;

        [Fact]
        public async Task GetGroups_DefaultUsage_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();

            var expected = _mapper.Map<List<GroupReadSummaryDTO>>(groups);

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act
            var result = (await service.GetGroupsSummaries(DEFAULT_OFFSET, DEFAULT_LIMIT, default)).ToList();

            // Assert
            Assert.Equal(expected.Count, result.Count);

            for (int i = 0; i < groups.Count; i++)
            {
                Assert.Equal(expected[i], result[i], new GroupComparer());
            }
        }

        [Theory]
        [InlineData(0, 5, 5)]
        [InlineData(0, 3, 3)]
        [InlineData(3, 5, 2)]
        [InlineData(6, 5, 0)]
        public async Task GetGroups_CorrectoffsetAndlimit_ReturnsCorrectResult(int offset, int limit, int expectedCount)
        {
            // Assign
            var groups = GetMockGroups();

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummaries(offset, limit, default);

            // Assert
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetGroups_OneGroupIsDeleted_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();

            var deletedId = 3;

            groups.Find(g => g.Id == deletedId).DeletedAt = DateTime.UtcNow;

            var expected = _mapper.Map<List<GroupReadSummaryDTO>>(groups.Where(g => g.Id != deletedId));

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act
            var result = (await service.GetGroupsSummaries(DEFAULT_OFFSET, DEFAULT_LIMIT, default)).ToList();

            // Assert
            Assert.Equal(expected.Count, result.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.Equal(expected[i], result[i], new GroupComparer());
            }
        }

        [Fact]
        public async Task GetGroupsByFilter_CorrectTitle_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();

            var searchedTitle = "Title 2";

            var expected = _mapper.Map<GroupReadSummaryDTO>(groups.Find(g => g.Title == searchedTitle));

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                Title = searchedTitle
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default);

            // Assert
            Assert.Equal(expected, result.ElementAt(0), new GroupComparer());
        }

        [Fact]
        public async Task GetGroupsByFilter_WrongTitle_ThrowsKeyNotFoundException()
        {
            // Assign
            var groups = GetMockGroups();

            var searchedTitle = "Title 8";

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                Title = searchedTitle
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default));
        }

        [Fact]
        public async Task GetGroupsByFilter_CorrectTeacherId_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();
            var teachers = GetMockTeachers();
            
            var groupTeacherInfos = GetMockGroupTeacherInfos(groups, teachers);

            var teacherId = 1;

            var expectedCount = groupTeacherInfos.Where(gt => gt.TeacherId == teacherId).Count();

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<GroupTeacher>())
                .ReturnsDbSet(groupTeacherInfos);
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);
            context.Setup(ctx => ctx.Set<Teacher>())
                .ReturnsDbSet(teachers);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TeacherId = teacherId
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default);

            // Assert
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetGroupsByFilter_CorrectTeacherIdAndOneGroupIsDeleted_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();
            var teachers = GetMockTeachers();

            var groupTeacherInfos = GetMockGroupTeacherInfos(groups, teachers);

            var teacherId = 1;

            groupTeacherInfos.First().Group.DeletedAt = DateTime.UtcNow;

            var expectedCount = groupTeacherInfos
                .Where(gt => gt.TeacherId == teacherId)
                .Count() - 1;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<GroupTeacher>())
                .ReturnsDbSet(groupTeacherInfos);
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);
            context.Setup(ctx => ctx.Set<Teacher>())
                .ReturnsDbSet(teachers);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TeacherId = teacherId
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default);

            // Assert
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetGroupsByFilter_NotExistingTeacherId_ThrowsArgumentException()
        {
            // Assign
            var groups = GetMockGroups();
            var teachers = GetMockTeachers();

            var groupTeacherInfos = GetMockGroupTeacherInfos(groups, teachers);

            var teacherId = 7;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<GroupTeacher>())
                .ReturnsDbSet(groupTeacherInfos);
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);
            context.Setup(ctx => ctx.Set<Teacher>())
                .ReturnsDbSet(teachers);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TeacherId = teacherId
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default));
        }

        [Fact]
        public async Task GetGroupsByFilter_DeletedTeacherId_ThrowsArgumentException()
        {
            // Assign
            var groups = GetMockGroups();
            var teachers = GetMockTeachers();

            var groupTeacherInfos = GetMockGroupTeacherInfos(groups, teachers);

            var teacherId = 3;

            groupTeacherInfos.First(gt => gt.TeacherId == teacherId).Teacher.DeletedAt = DateTime.UtcNow;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<GroupTeacher>())
                .ReturnsDbSet(groupTeacherInfos);
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);
            context.Setup(ctx => ctx.Set<Teacher>())
                .ReturnsDbSet(teachers);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TeacherId = teacherId
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default));
        }

        [Fact]
        public async Task GetGroupsByFilter_CorrectTeacherIdAndExistingTitlePattern_ReturnsOneItem()
        {
            // Assign
            var groups = GetMockGroups();
            var teachers = GetMockTeachers();

            var groupTeacherInfos = GetMockGroupTeacherInfos(groups, teachers);

            var teacherId = 1;
            var titlePattern = "2";

            var expectedCount = 1;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<GroupTeacher>())
                .ReturnsDbSet(groupTeacherInfos);
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);
            context.Setup(ctx => ctx.Set<Teacher>())
                .ReturnsDbSet(teachers);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TeacherId = teacherId,
                TitlePattern = titlePattern
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default);

            // Assert
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetGroupsByFilter_CorrectTeacherIdAndNotExistingTitlePattern_ReturnsEmptyList()
        {
            // Assign
            var groups = GetMockGroups();
            var teachers = GetMockTeachers();

            var groupTeacherInfos = GetMockGroupTeacherInfos(groups, teachers);

            var teacherId = 1;
            var titlePattern = "6";

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<GroupTeacher>())
                .ReturnsDbSet(groupTeacherInfos);
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);
            context.Setup(ctx => ctx.Set<Teacher>())
                .ReturnsDbSet(teachers);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TeacherId = teacherId,
                TitlePattern = titlePattern
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupsByFilter_ExistingTitlePattern_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();

            var titlePattern = "2";

            var expectedCount = 1;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TitlePattern = titlePattern,
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default);

            // Assert
            Assert.Equal(expectedCount, result.Count());
        }

        [Fact]
        public async Task GetGroupsByFilter_NotExistingTitlePattern_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();

            var titlePattern = "6";

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupsSummariesByFilter(new GroupFilter()
            {
                TitlePattern = titlePattern,
            }, DEFAULT_OFFSET, DEFAULT_LIMIT, default);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupById_CorrectGroupId_ReturnsCorrectResult()
        {
            // Assign
            var groups = GetMockGroups();

            var groupId = groups.First().Id;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act
            var result = await service.GetGroupSummaryById(groupId);

            // Assert
            Assert.Equal(_mapper.Map<GroupReadSummaryDTO>(groups[0]), result, new GroupComparer());
        }

        [Fact]
        public async Task GetGroupById_WrongGroupId_ThrowsKeyNotFoundException()
        {
            // Assign
            var groups = GetMockGroups();

            var groupId = groups.Count() + 1;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetGroupSummaryById(groupId));
        }

        [Fact]
        public async Task AddGroup_AlreadylimitnTitle_ThrowsArgumentException()
        {
            // Assign
            var groups = GetMockGroups();

            var createDTO = new GroupCreateDTO()
            {
                Title = groups.First().Title,
            };

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.AddGroup(createDTO));
        }

        [Fact]
        public async Task UpdateGroupTitle_NotExistingGroupId_ThrowsKeyNotFoundException()
        {
            // Assign
            var groups = GetMockGroups();

            var id = groups.Count() + 1;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.UpdateGroup(id, new GroupUpdateDTO() { Title = string.Empty }, default));
        }

        [Fact]
        public async Task UpdateGroupTitle_AlreadylimitnTitle_ThrowsArgumentException()
        {
            // Assign
            var groups = GetMockGroups();

            var id = groups.First().Id;
            var title = groups.First().Title;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateGroup(id, new GroupUpdateDTO() { Title = title }, default));
        }

        [Fact]
        public async Task DeleteGroup_NotExistingGroupId_ThrowsKeyNotFoundException()
        {
            // Assign
            var groups = GetMockGroups();

            var id = groups.Count + 1;

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<Group>())
                .ReturnsDbSet(groups);

            var service = GetGroupService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.DeleteGroup(id));
        }

        private List<Group> GetMockGroups(int size = 5)
        {
            var result = new List<Group>();

            for (int i = 1; i <= size; ++i)
            {
                result.Add(new Group()
                {
                    Id = i,
                    Title = $"Title {i}"
                });
            }

            return result;
        }

        private List<Teacher> GetMockTeachers(int size = 5)
        {
            var result = new List<Teacher>();

            for (int i = 1; i <= size; ++i)
            {
                result.Add(new Teacher()
                {
                    Id = i,
                    Username = $"Username {i}",
                    Password = $"Password {i}",
                    FirstName = $"FirstName {i}",
                    LastName = $"LastName {i}",
                    Age = 20
                });
            }

            return result;
        }

        private List<GroupTeacher> GetMockGroupTeacherInfos(List<Group> groups, List<Teacher> teachers)
        {
            var result = new List<GroupTeacher>();

            for (int i = 0; i < groups.Count; ++i)
            {
                for (int j = 0; j < teachers.Count; ++j)
                {
                    result.Add(new GroupTeacher()
                    {
                        Id = (i * 5) + j + 1,
                        GroupId = i + 1,
                        Group = groups[i],
                        TeacherId = j + 1,
                        Teacher = teachers[j]
                    });
                }
            }

            return result;
        }

        private GroupService GetGroupService(DbContext context)
        {
            var repository = new MockRepository(context);

            var dataHelper = new MockDataHelper(repository);

            return new GroupService(repository, _mapper, dataHelper);
        }
    }
}
