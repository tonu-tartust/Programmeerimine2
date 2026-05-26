using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Projects;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Projects
{
    public class ProjectsQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_results()
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

            var query = new ProjectsQuery { Page = 1, PageSize = 10 };
            var handler = new ProjectsQueryHandler(DbContext);

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
            var p1 = new Project { Name = "Alpha", Description = "Desc1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Status = "Active" };
            var p2 = new Project { Name = "Beta", Description = "Desc2", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(1), Status = "Active" };
            await DbContext.Projects.AddRangeAsync(p1, p2);
            await DbContext.SaveChangesAsync();

            var query = new ProjectsQuery { Keyword = "Alpha", Page = 1, PageSize = 10 };
            var handler = new ProjectsQueryHandler(DbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value.Results);
            Assert.Equal("Alpha", result.Value.Results[0].Name);
        }

        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            // Arrange
            var handler = new ProjectsQueryHandler(DbContext);

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
            var query = new ProjectsQuery { Page = page, PageSize = 10 };
            var handler = new ProjectsQueryHandler(dbContext);

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
            var query = new ProjectsQuery { Page = 1, PageSize = pageSize };
            var handler = new ProjectsQueryHandler(dbContext);

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
            var query = new ProjectsQuery { Page = 1, PageSize = pageSize };
            var handler = new ProjectsQueryHandler(dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }
    }
}
