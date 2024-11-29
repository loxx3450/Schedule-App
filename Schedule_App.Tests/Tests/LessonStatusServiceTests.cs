using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using Schedule_App.API;
using Schedule_App.API.DTOs.LessonStatus;
using Schedule_App.API.Services;
using Schedule_App.Core.Models;
using Schedule_App.Tests.Comparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Schedule_App.Tests.Tests
{
    public class LessonStatusServiceTests : BaseTestsClass
    {
        [Fact]
        public async Task GetLessonStatuses_CorrectInput_ReturnsCorrectResult()
        {
            // Assign
            var lessonStatuses = GetMockLessonStatuses();

            var expected = _mapper.Map<List<LessonStatusReadDTO>>(lessonStatuses);

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<LessonStatus>())
                .ReturnsDbSet(lessonStatuses);

            var service = GetLessonStatusService(context.Object);

            // Act
            var result = (await service.GetLessonStatuses(default)).ToList();

            // Assert
            Assert.Equal(expected.Count, result.Count);

            for (int i = 0; i < lessonStatuses.Count; i++)
            {
                Assert.Equal(expected[i], result[i], new LessonStatusComparer());
            }
        }

        [Fact]
        public async Task GetLessonStatusById_CorrectInput_ReturnsCorrectResult()
        {
            // Assign
            var lessonStatuses = GetMockLessonStatuses();

            var searchedId = 2;
            var expected = _mapper.Map<LessonStatusReadDTO>(lessonStatuses.Find(ls => ls.Id == searchedId));

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<LessonStatus>())
                .ReturnsDbSet(lessonStatuses);

            var service = GetLessonStatusService(context.Object);

            // Act
            var result = await service.GetLessonStatusById(searchedId, default);

            // Assert
            Assert.Equal(expected, result, new LessonStatusComparer());
        }

        [Fact]
        public async Task GetLessonStatusById_NotExistingIndex_ThrowsKeyNotFoundException()
        {
            // Assign
            var lessonStatuses = GetMockLessonStatuses();

            var searchedId = lessonStatuses.Count + 1;
            var expected = _mapper.Map<LessonStatusReadDTO>(lessonStatuses.Find(ls => ls.Id == searchedId));

            var context = new Mock<DbContext>();
            context.Setup(ctx => ctx.Set<LessonStatus>())
                .ReturnsDbSet(lessonStatuses);

            var service = GetLessonStatusService(context.Object);

            // Act / Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await service.GetLessonStatusById(searchedId, default));
        }

        private List<LessonStatus> GetMockLessonStatuses()
        {
            var result = new List<LessonStatus>();

            for (int i = 1; i <= 5; ++i)
            {
                result.Add(new LessonStatus()
                {
                    Id = i,
                    Description = $"Description {i}"
                });
            }

            return result;
        }

        private LessonStatusService GetLessonStatusService(DbContext context)
        {
            var repository = new MockRepository(context);

            var dataHelper = new MockDataHelper(repository);

            return new LessonStatusService(repository, _mapper, dataHelper);
        }
    }
}
