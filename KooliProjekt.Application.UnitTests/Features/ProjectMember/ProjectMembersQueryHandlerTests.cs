using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.ProjectMembers;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.ProjectMembers
{
    public class ProjectMembersQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_results()
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

            var query = new ProjectMembersQuery { Page = 1, PageSize = 10 };
            var handler = new ProjectMembersQueryHandler(DbContext);

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
            var pm1 = new ProjectMember { ProjectId = 1, EmployeeId = 1, RoleInProject = "Developer" };
            var pm2 = new ProjectMember { ProjectId = 1, EmployeeId = 2, RoleInProject = "Tester" };
            await DbContext.ProjectMembers.AddRangeAsync(pm1, pm2);
            await DbContext.SaveChangesAsync();

            var query = new ProjectMembersQuery { Keyword = "Tester", Page = 1, PageSize = 10 };
            var handler = new ProjectMembersQueryHandler(DbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value.Results);
            Assert.Equal("Tester", result.Value.Results[0].RoleInProject);
        }

        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            // Arrange
            var handler = new ProjectMembersQueryHandler(DbContext);

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
            var query = new ProjectMembersQuery { Page = page, PageSize = 10 };
            var handler = new ProjectMembersQueryHandler(dbContext);

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
            var query = new ProjectMembersQuery { Page = 1, PageSize = pageSize };
            var handler = new ProjectMembersQueryHandler(dbContext);

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
            var query = new ProjectMembersQuery { Page = 1, PageSize = pageSize };
            var handler = new ProjectMembersQueryHandler(dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }
    }
}
