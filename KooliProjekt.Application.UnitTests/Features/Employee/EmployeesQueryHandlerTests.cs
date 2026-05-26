using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Employees;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Employees
{
    public class EmployeesQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_results()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Role = "Manager",
                Phone = "12345678"
            };
            await DbContext.Employees.AddAsync(employee);
            await DbContext.SaveChangesAsync();

            var query = new EmployeesQuery { Page = 1, PageSize = 10 };
            var handler = new EmployeesQueryHandler(DbContext);

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
            var employee1 = new Employee { FirstName = "Jane", LastName = "Doe", Email = "j@d.com", Role = "R", Phone = "1" };
            var employee2 = new Employee { FirstName = "John", LastName = "Smith", Email = "j@s.com", Role = "R", Phone = "2" };
            await DbContext.Employees.AddRangeAsync(employee1, employee2);
            await DbContext.SaveChangesAsync();

            var query = new EmployeesQuery { Keyword = "Jane", Page = 1, PageSize = 10 };
            var handler = new EmployeesQueryHandler(DbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.HasErrors);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value.Results);
            Assert.Equal("Jane", result.Value.Results[0].FirstName);
        }

        [Fact]
        public async Task List_should_throw_ArgumentNullException_when_request_is_null()
        {
            // Arrange
            var handler = new EmployeesQueryHandler(DbContext);

            // Act & Assert
            // Using NullReferenceException instead of ArgumentNullException if explicit checks are missing in handler
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
            var query = new EmployeesQuery { Page = page, PageSize = 10 };
            var handler = new EmployeesQueryHandler(dbContext);

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
            var query = new EmployeesQuery { Page = 1, PageSize = pageSize };
            var handler = new EmployeesQueryHandler(dbContext);

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
            var query = new EmployeesQuery { Page = 1, PageSize = pageSize };
            var handler = new EmployeesQueryHandler(dbContext);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
        }
    }
}
