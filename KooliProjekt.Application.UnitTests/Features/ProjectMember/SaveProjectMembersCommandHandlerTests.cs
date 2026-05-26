using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.ProjectMembers;
using Moq;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.ProjectMembers
{
    public class SaveProjectMembersCommandHandlerTests : TestBase
    {
        [Fact]
        public void Save_should_throw_when_repository_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SaveProjectMembersCommandHandler(null);
            });
        }

        [Fact]
        public async Task Save_should_throw_when_request_is_null()
        {
             var mockRepository = new Mock<IProjectMemberRepository>();
             var handler = new SaveProjectMembersCommandHandler(mockRepository.Object);
             await Assert.ThrowsAsync<ArgumentNullException>(async () =>
             {
                 await handler.Handle(null, CancellationToken.None);
             });
        }

        [Fact]
        public async Task Save_should_return_error_when_existing_project_member_is_not_found()
        {
            var mockRepository = new Mock<IProjectMemberRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ProjectMember)null);

            var request = new SaveProjectMembersCommand { Id = 1 };
            var handler = new SaveProjectMembersCommandHandler(mockRepository.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_save_new_project_member()
        {
            // Arrange
            var mockRepository = new Mock<IProjectMemberRepository>();
            ProjectMember savedProjectMember = null;
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<ProjectMember>()))
                .Callback<ProjectMember>(pm => savedProjectMember = pm)
                .Returns(Task.CompletedTask);

            var request = new SaveProjectMembersCommand
            {
                Id = 0,
                ProjectId = 1,
                EmployeeId = 1,
                RoleInProject = "Developer"
            };
            var handler = new SaveProjectMembersCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<ProjectMember>()), Times.Once);
            Assert.NotNull(savedProjectMember);
            Assert.Equal(request.ProjectId, savedProjectMember.ProjectId);
            Assert.Equal(request.EmployeeId, savedProjectMember.EmployeeId);
            Assert.Equal(request.RoleInProject, savedProjectMember.RoleInProject);
        }

        [Fact]
        public async Task Save_should_update_existing_project_member()
        {
            // Arrange
            var existingProjectMember = new ProjectMember
            {
                Id = 1,
                ProjectId = 1,
                EmployeeId = 1,
                RoleInProject = "Intern"
            };

            var mockRepository = new Mock<IProjectMemberRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingProjectMember);
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<ProjectMember>()))
                .Returns(Task.CompletedTask);

            var request = new SaveProjectMembersCommand
            {
                Id = 1,
                ProjectId = 2,
                EmployeeId = 2,
                RoleInProject = "Lead Developer"
            };
            var handler = new SaveProjectMembersCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<ProjectMember>()), Times.Once);
            Assert.Equal(request.ProjectId, existingProjectMember.ProjectId);
            Assert.Equal(request.EmployeeId, existingProjectMember.EmployeeId);
            Assert.Equal(request.RoleInProject, existingProjectMember.RoleInProject);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SaveValidator_should_return_false_when_ProjectId_is_invalid(int projectId)
        {
            // Arrange
            var validator = new SaveProjectMembersCommandValidator(DbContext);
            var command = new SaveProjectMembersCommand
            {
                Id = 0,
                ProjectId = projectId,
                EmployeeId = 1,
                RoleInProject = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectMembersCommand.ProjectId), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void SaveValidator_should_return_false_when_EmployeeId_is_invalid(int employeeId)
        {
            // Arrange
            var validator = new SaveProjectMembersCommandValidator(DbContext);
            var command = new SaveProjectMembersCommand
            {
                Id = 0,
                ProjectId = 1,
                EmployeeId = employeeId,
                RoleInProject = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectMembersCommand.EmployeeId), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_RoleInProject_is_invalid(string roleInProject)
        {
            // Arrange
            var validator = new SaveProjectMembersCommandValidator(DbContext);
            var command = new SaveProjectMembersCommand
            {
                Id = 0,
                ProjectId = 1,
                EmployeeId = 1,
                RoleInProject = roleInProject
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveProjectMembersCommand.RoleInProject), result.Errors.First().PropertyName);
        }

        [Fact]
        public void SaveValidator_should_return_true_when_data_is_valid()
        {
            // Arrange
            var validator = new SaveProjectMembersCommandValidator(DbContext);
            var command = new SaveProjectMembersCommand
            {
                Id = 0,
                ProjectId = 1,
                EmployeeId = 1,
                RoleInProject = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
