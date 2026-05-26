using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Projects;
using Moq;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Projects
{
    public class SaveProjectsCommandHandlerTests : TestBase
    {
        [Fact]
        public void Save_should_throw_when_repository_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SaveProjectsCommandHandler(null);
            });
        }

        [Fact]
        public async Task Save_should_throw_when_request_is_null()
        {
             var mockRepository = new Mock<IProjectRepository>();
             var handler = new SaveProjectsCommandHandler(mockRepository.Object);
             await Assert.ThrowsAsync<ArgumentNullException>(async () =>
             {
                 await handler.Handle(null, CancellationToken.None);
             });
        }

        [Fact]
        public async Task Save_should_return_error_when_existing_project_is_not_found()
        {
            var mockRepository = new Mock<IProjectRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Project)null);

            var request = new SaveProjectsCommand { Id = 1 };
            var handler = new SaveProjectsCommandHandler(mockRepository.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_save_new_project()
        {
            // Arrange
            var mockRepository = new Mock<IProjectRepository>();
            Project savedProject = null;
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<Project>()))
                .Callback<Project>(p => savedProject = p)
                .Returns(Task.CompletedTask);

            var request = new SaveProjectsCommand
            {
                Id = 0,
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                Status = "Active",
                Budget = 10000m
            };
            var handler = new SaveProjectsCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<Project>()), Times.Once);
            Assert.NotNull(savedProject);
            Assert.Equal(request.Name, savedProject.Name);
            Assert.Equal(request.Description, savedProject.Description);
            Assert.Equal(request.Status, savedProject.Status);
            Assert.Equal(request.Budget, savedProject.Budget);
        }

        [Fact]
        public async Task Save_should_update_existing_project()
        {
            // Arrange
            var existingProject = new Project
            {
                Id = 1,
                Name = "Old Name",
                Description = "Old Description",
                StartDate = DateTime.Now.AddDays(-10),
                EndDate = DateTime.Now.AddDays(20),
                Status = "Pending",
                Budget = 5000m
            };

            var mockRepository = new Mock<IProjectRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingProject);
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<Project>()))
                .Returns(Task.CompletedTask);

            var request = new SaveProjectsCommand
            {
                Id = 1,
                Name = "Updated Project",
                Description = "Updated Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(60),
                Status = "Active",
                Budget = 20000m
            };
            var handler = new SaveProjectsCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<Project>()), Times.Once);
            Assert.Equal(request.Name, existingProject.Name);
            Assert.Equal(request.Description, existingProject.Description);
            Assert.Equal(request.Status, existingProject.Status);
            Assert.Equal(request.Budget, existingProject.Budget);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Name_is_invalid(string name)
        {
            // Arrange
            var validator = new SaveProjectsCommandValidator(DbContext);
            var command = new SaveProjectsCommand
            {
                Id = 0,
                Name = name,
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                Status = "Active",
                Budget = 10000m
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectsCommand.Name), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Description_is_invalid(string description)
        {
            // Arrange
            var validator = new SaveProjectsCommandValidator(DbContext);
            var command = new SaveProjectsCommand
            {
                Id = 0,
                Name = "Test Project",
                Description = description,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                Status = "Active",
                Budget = 10000m
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectsCommand.Description), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Status_is_invalid(string status)
        {
            // Arrange
            var validator = new SaveProjectsCommandValidator(DbContext);
            var command = new SaveProjectsCommand
            {
                Id = 0,
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                Status = status,
                Budget = 10000m
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectsCommand.Status), result.Errors.First().PropertyName);
        }

        [Fact]
        public void SaveValidator_should_return_false_when_EndDate_is_before_StartDate()
        {
            // Arrange
            var validator = new SaveProjectsCommandValidator(DbContext);
            var command = new SaveProjectsCommand
            {
                Id = 0,
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.Now.AddDays(30),
                EndDate = DateTime.Now,
                Status = "Active",
                Budget = 10000m
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectsCommand.EndDate), result.Errors.First().PropertyName);
        }

        [Fact]
        public void SaveValidator_should_return_false_when_Budget_is_negative()
        {
            // Arrange
            var validator = new SaveProjectsCommandValidator(DbContext);
            var command = new SaveProjectsCommand
            {
                Id = 0,
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                Status = "Active",
                Budget = -100m
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectsCommand.Budget), result.Errors.First().PropertyName);
        }

        [Fact]
        public void SaveValidator_should_return_true_when_data_is_valid()
        {
            // Arrange
            var validator = new SaveProjectsCommandValidator(DbContext);
            var command = new SaveProjectsCommand
            {
                Id = 0,
                Name = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                Status = "Active",
                Budget = 10000m
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
