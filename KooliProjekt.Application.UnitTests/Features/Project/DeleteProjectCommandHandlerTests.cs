using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Projects;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Projects
{
    public class DeleteProjectCommandHandlerTests : TestBase
    {
        [Fact]
        public void Delete_should_throw_when_dbcontext_is_null()
        {
            var dbContext = (ApplicationDbContext)null;
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                new DeleteProjectCommandHandler(dbContext);
            });

            Assert.Equal(nameof(dbContext), exception.ParamName);
        }

        [Fact]
        public async Task Delete_should_throw_when_request_is_null()
        {
            // Arrange
            var request = (DeleteProjectCommand)null;
            var handler = new DeleteProjectCommandHandler(DbContext);

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
            var command = new DeleteProjectCommand { Id = id };
            var handler = new DeleteProjectCommandHandler(DbContext);

            var project = new Project
            {
                Name = "Test Project",
                Description = "Test Desc",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                Status = "Active"
            };
            await DbContext.Projects.AddAsync(project);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_remove_existing_project()
        {
            // Arrange
            var project = new Project
            {
                Name = "Test Project",
                Description = "Test Desc",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                Status = "Active"
            };
            await DbContext.Projects.AddAsync(project);
            await DbContext.SaveChangesAsync();

            var command = new DeleteProjectCommand { Id = project.Id };
            var handler = new DeleteProjectCommandHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var deletedProject = await DbContext.Projects.FindAsync(command.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(deletedProject);
        }

        [Fact]
        public async Task Delete_should_not_fail_when_project_does_not_exist()
        {
            // Arrange
            var project = new Project
            {
                Name = "Test Project",
                Description = "Test Desc",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                Status = "Active"
            };
            await DbContext.Projects.AddAsync(project);
            await DbContext.SaveChangesAsync();

            var command = new DeleteProjectCommand { Id = 999 };
            var handler = new DeleteProjectCommandHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var existingProject = await DbContext.Projects.FindAsync(project.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.NotNull(existingProject);
        }
    }
}
