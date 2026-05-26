using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Task;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Tasks
{
    public class GetTasksQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_object_if_object_exists()
        {
            // Arrange
            var taskEntity = new KooliProjekt.Application.Data.Tasks
            {
                ProjectId = 1,
                Title = "Test Task",
                Description = "Description",
                Status = "New",
                Priority = "High",
                DueDate = DateTime.Now.AddDays(1)
            };
            await DbContext.Set<KooliProjekt.Application.Data.Tasks>().AddAsync(taskEntity);
            await DbContext.SaveChangesAsync();

            var query = new GetTasksQuery { Id = taskEntity.Id };
            var handler = new GetTasksQueryHandler(DbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
        }

        [Fact]
        public async Task Get_should_return_null_if_object_does_not_exist()
        {
            // Arrange
            var taskEntity = new KooliProjekt.Application.Data.Tasks
            {
                ProjectId = 1,
                Title = "Test Task 2",
                Description = "Description",
                Status = "New",
                Priority = "High",
                DueDate = DateTime.Now.AddDays(1)
            };
            await DbContext.Set<KooliProjekt.Application.Data.Tasks>().AddAsync(taskEntity);
            await DbContext.SaveChangesAsync();

            var query = new GetTasksQuery { Id = 999 };
            var handler = new GetTasksQueryHandler(DbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }

        [Fact]
        public async Task Get_should_throw_ArgumentNullException_when_request_is_null()
        {
            // Arrange
            var handler = new GetTasksQueryHandler(DbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => handler.Handle(null, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public async Task Get_should_return_null_when_request_id_is_zero_or_less(int id)
        {
            // Arrange
            var dbContext = GetFaultyDbContext();
            var query = new GetTasksQuery { Id = id };
            var handler = new GetTasksQueryHandler(dbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }
    }
}
