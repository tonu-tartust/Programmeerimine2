using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.ProjectMembers;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.ProjectMembers
{
    public class GetProjectMembersQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_object_if_object_exists()
        {
            // Arrange
            var projectMember = new ProjectMember
            {
                ProjectId = 1,
                EmployeeId = 1,
                RoleInProject = "Developer"
            };
            await DbContext.ProjectMembers.AddAsync(projectMember);
            await DbContext.SaveChangesAsync();

            var query = new GetProjectMembersQuery { Id = projectMember.Id };
            var handler = new GetProjectMembersQueryHandler(DbContext);

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
            var projectMember = new ProjectMember
            {
                ProjectId = 1,
                EmployeeId = 1,
                RoleInProject = "Manager"
            };
            await DbContext.ProjectMembers.AddAsync(projectMember);
            await DbContext.SaveChangesAsync();

            var query = new GetProjectMembersQuery { Id = 999 };
            var handler = new GetProjectMembersQueryHandler(DbContext);

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
            var handler = new GetProjectMembersQueryHandler(DbContext);

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
            var query = new GetProjectMembersQuery { Id = id };
            var handler = new GetProjectMembersQueryHandler(dbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }
    }
}
