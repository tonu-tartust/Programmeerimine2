using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Employees;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Employees
{
    public class GetEmployeesQueryHandlerTests : TestBase
    {
        [Fact]
        public async Task Get_should_return_object_if_object_exists()
        {
            // Arrange
            var employee = new Employee
            {
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@example.com",
                Role = "Developer",
                Phone = "1234567890"
            };
            await DbContext.Employees.AddAsync(employee);
            await DbContext.SaveChangesAsync();

            var query = new GetEmployeesQuery { Id = employee.Id };
            var handler = new GetEmployeesQueryHandler(DbContext);

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
            var employee = new Employee
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Role = "Manager",
                Phone = "0987654321"
            };
            await DbContext.Employees.AddAsync(employee);
            await DbContext.SaveChangesAsync();

            var query = new GetEmployeesQuery { Id = 999 };
            var handler = new GetEmployeesQueryHandler(DbContext);

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
            var handler = new GetEmployeesQueryHandler(DbContext);

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
            var query = new GetEmployeesQuery { Id = id };
            var handler = new GetEmployeesQueryHandler(dbContext);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(result.Value);
        }
    }
}
