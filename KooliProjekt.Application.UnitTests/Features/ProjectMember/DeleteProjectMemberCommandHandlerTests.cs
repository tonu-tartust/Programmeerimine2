using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.ProjectMembers;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.ProjectMembers
{
    public class DeleteProjectMemberCommandHandlerTests : TestBase
    {
        [Fact]
        public void Delete_should_throw_when_dbcontext_is_null()
        {
            var dbContext = (ApplicationDbContext)null;
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                new DeleteProjectMemberCommandHandler(dbContext);
            });

            Assert.Equal(nameof(dbContext), exception.ParamName);
        }

        [Fact]
        public async Task Delete_should_throw_when_request_is_null()
        {
            // Arrange
            var request = (DeleteProjectMemberCommand)null;
            var handler = new DeleteProjectMemberCommandHandler(DbContext);

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
            var command = new DeleteProjectMemberCommand { Id = id };
            var handler = new DeleteProjectMemberCommandHandler(DbContext);

            var pm = new ProjectMember
            {
                ProjectId = 1,
                EmployeeId = 2,
                RoleInProject = "Developer"
            };
            await DbContext.ProjectMembers.AddAsync(pm);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_remove_existing_project_member()
        {
            // Arrange
            var pm = new ProjectMember
            {
                ProjectId = 1,
                EmployeeId = 2,
                RoleInProject = "Developer"
            };
            await DbContext.ProjectMembers.AddAsync(pm);
            await DbContext.SaveChangesAsync();

            var command = new DeleteProjectMemberCommand { Id = pm.Id };
            var handler = new DeleteProjectMemberCommandHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var deletedPm = await DbContext.ProjectMembers.FindAsync(command.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(deletedPm);
        }

        [Fact]
        public async Task Delete_should_not_fail_when_project_member_does_not_exist()
        {
            // Arrange
            var pm = new ProjectMember
            {
                ProjectId = 1,
                EmployeeId = 2,
                RoleInProject = "Developer"
            };
            await DbContext.ProjectMembers.AddAsync(pm);
            await DbContext.SaveChangesAsync();

            var command = new DeleteProjectMemberCommand { Id = 999 };
            var handler = new DeleteProjectMemberCommandHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var existingPm = await DbContext.ProjectMembers.FindAsync(pm.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.NotNull(existingPm);
        }
    }
}
