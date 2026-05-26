using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Task;
using Moq;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Tasks
{
    public class SaveTasksCommandHandlerTests : TestBase
    {
        [Fact]
        public void Save_should_throw_when_repository_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SaveTasksCommandHandler(null);
            });
        }

        [Fact]
        public async Task Save_should_throw_when_request_is_null()
        {
            // Arrange
            var mockRepository = new Mock<ITaskRepository>();
            var handler = new SaveTasksCommandHandler(mockRepository.Object);

            // Act && Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await handler.Handle(null, CancellationToken.None);
            });
        }

        [Fact]
        public async Task Save_should_return_error_when_existing_task_is_not_found()
        {
            // Arrange
            var mockRepository = new Mock<ITaskRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Data.Tasks)null);

            var request = new SaveTasksCommand { Id = 1 };
            var handler = new SaveTasksCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_save_new_task()
        {
            // Arrange
            var mockRepository = new Mock<ITaskRepository>();
            Data.Tasks savedTask = null;
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<Data.Tasks>()))
                .Callback<Data.Tasks>(t => savedTask = t)
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var request = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = "Test Task",
                Description = "Test Description",
                AssignedTo = 1,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Open",
                Priority = "High"
            };
            var handler = new SaveTasksCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<Data.Tasks>()), Times.Once);
            Assert.NotNull(savedTask);
            Assert.Equal(request.ProjectId, savedTask.ProjectId);
            Assert.Equal(request.Title, savedTask.Title);
            Assert.Equal(request.Description, savedTask.Description);
            Assert.Equal(request.AssignedTo, savedTask.AssignedTo);
            Assert.Equal(request.Status, savedTask.Status);
            Assert.Equal(request.Priority, savedTask.Priority);
        }

        [Fact]
        public async Task Save_should_update_existing_task()
        {
            // Arrange
            var existingTask = new Data.Tasks
            {
                Id = 1,
                ProjectId = 1,
                Title = "Old Task",
                Description = "Old Description",
                AssignedTo = 1,
                StartDate = DateTime.Now.AddDays(-5),
                DueDate = DateTime.Now.AddDays(5),
                Status = "Open",
                Priority = "Low"
            };

            var mockRepository = new Mock<ITaskRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingTask);
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<Data.Tasks>()))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var request = new SaveTasksCommand
            {
                Id = 1,
                ProjectId = 2,
                Title = "Updated Task",
                Description = "Updated Description",
                AssignedTo = 2,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14),
                Status = "In Progress",
                Priority = "High"
            };
            var handler = new SaveTasksCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<Data.Tasks>()), Times.Once);
            Assert.Equal(request.ProjectId, existingTask.ProjectId);
            Assert.Equal(request.Title, existingTask.Title);
            Assert.Equal(request.Description, existingTask.Description);
            Assert.Equal(request.AssignedTo, existingTask.AssignedTo);
            Assert.Equal(request.Status, existingTask.Status);
            Assert.Equal(request.Priority, existingTask.Priority);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SaveValidator_should_return_false_when_ProjectId_is_invalid(int projectId)
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = projectId,
                Title = "Test Task",
                Description = "Test Description",
                AssignedTo = 1,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Open",
                Priority = "High"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveTasksCommand.ProjectId), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Title_is_invalid(string title)
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = title,
                Description = "Test Description",
                AssignedTo = 1,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Open",
                Priority = "High"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveTasksCommand.Title), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Description_is_invalid(string description)
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = "Test Task",
                Description = description,
                AssignedTo = 1,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Open",
                Priority = "High"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveTasksCommand.Description), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SaveValidator_should_return_false_when_AssignedTo_is_invalid(int assignedTo)
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = "Test Task",
                Description = "Test Description",
                AssignedTo = assignedTo,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Open",
                Priority = "High"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveTasksCommand.AssignedTo), result.Errors.First().PropertyName);
        }

        [Fact]
        public void SaveValidator_should_return_false_when_DueDate_is_before_StartDate()
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = "Test Task",
                Description = "Test Description",
                AssignedTo = 1,
                StartDate = DateTime.Now.AddDays(7),
                DueDate = DateTime.Now,
                Status = "Open",
                Priority = "High"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveTasksCommand.DueDate), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Status_is_invalid(string status)
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = "Test Task",
                Description = "Test Description",
                AssignedTo = 1,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = status,
                Priority = "High"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveTasksCommand.Status), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Priority_is_invalid(string priority)
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = "Test Task",
                Description = "Test Description",
                AssignedTo = 1,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Open",
                Priority = priority
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveTasksCommand.Priority), result.Errors.First().PropertyName);
        }

        [Fact]
        public void SaveValidator_should_return_true_when_data_is_valid()
        {
            // Arrange
            var validator = new SaveTasksCommandValidator(DbContext);
            var command = new SaveTasksCommand
            {
                Id = 0,
                ProjectId = 1,
                Title = "Test Task",
                Description = "Test Description",
                AssignedTo = 1,
                StartDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(7),
                Status = "Open",
                Priority = "High"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
