using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Task;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Tasks
{
    public class DeleteTaskCommandHandlerTests : TestBase
    {
        [Fact]
        public void Delete_should_throw_when_dbcontext_is_null()
        {
            var dbContext = (ApplicationDbContext)null;
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                new DeleteTaskCommandHandler(dbContext);
            });

            Assert.Equal(nameof(dbContext), exception.ParamName);
        }

        [Fact]
        public async Task Delete_should_throw_when_request_is_null()
        {
            // Arrange
            var request = (DeleteTaskCommand)null;
            var handler = new DeleteTaskCommandHandler(DbContext);

            // Act && Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await handler.Handle(request, CancellationToken.None);
            });
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Delete_should_return_when_request_id_is_null_or_negative(int id)
        {
            // Arrange
            var command = new DeleteTaskCommand { Id = id };
            var handler = new DeleteTaskCommandHandler(DbContext);

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

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_remove_existing_task()
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

            var command = new DeleteTaskCommand { Id = taskData.Id };
            var handler = new DeleteTaskCommandHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var deletedTask = await DbContext.Taskss.FindAsync(command.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(deletedTask);
        }

        [Fact]
        public async Task Delete_should_not_fail_when_task_does_not_exist()
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

            var command = new DeleteTaskCommand { Id = 999 };
            var handler = new DeleteTaskCommandHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var existingTask = await DbContext.Taskss.FindAsync(taskData.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.NotNull(existingTask);
        }
    }
}
