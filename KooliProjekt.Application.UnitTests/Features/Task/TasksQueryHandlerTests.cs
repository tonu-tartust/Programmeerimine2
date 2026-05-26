using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Task;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Tasks
{
    public class TasksQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_results()
        {
            // Arrange
            var taskData = new KooliProjekt.Application.Data.Tasks
            {
                ProjectId = 1,
                Title = "Title",
                Description = "Desc",
                AssignedTo = 1,
                StartDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(10),
                Status = "Open",
                Priority = "High"
            };
            await DbContext.Taskss.AddAsync(taskData);
            await DbContext.SaveChangesAsync();

            var query = new TasksQuery { Page = 1, PageSize = 10 };
            var handler = new TasksQueryHandler(DbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.Results.Count > 0);
        }

        [Fact]
        public async Task List_should_filter_by_keyword()
        {
            // Arrange
            var t1 = new Data.Tasks { Title = "Task A", Description = "...", ProjectId = 1, AssignedTo = 1, StartDate = DateTime.Now, DueDate = DateTime.Now, Status = "O", Priority = "H" };
            var t2 = new Data.Tasks { Title = "Task B", Description = "...", ProjectId = 1, AssignedTo = 1, StartDate = DateTime.Now, DueDate = DateTime.Now, Status = "O", Priority = "H" };
            await DbContext.Taskss.AddRangeAsync(t1, t2);
            await DbContext.SaveChangesAsync();

            var query = new TasksQuery { Keyword = "A", Page = 1, PageSize = 10 };
            var handler = new TasksQueryHandler(DbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value.Results);
            Assert.Equal("Task A", result.Value.Results[0].Title);
        }

        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            // Arrange
            var handler = new TasksQueryHandler(DbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_page_is_zero_or_less(int page)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var query = new TasksQuery { Page = page, PageSize = 10 };
            var handler = new TasksQueryHandler(dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async Task List_should_throw_ArgumentException_when_pageSize_is_zero_or_less(int pageSize)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var query = new TasksQuery { Page = 1, PageSize = pageSize };
            var handler = new TasksQueryHandler(dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }

        [Theory]
        [InlineData(101)]
        [InlineData(200)]
        [InlineData(1000)]
        public async Task List_should_throw_ArgumentException_when_pageSize_exceeds_max(int pageSize)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var query = new TasksQuery { Page = 1, PageSize = pageSize };
            var handler = new TasksQueryHandler(dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }
    }
}
