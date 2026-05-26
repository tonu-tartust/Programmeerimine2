using System;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Employees;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Employees
{
    public class DeleteEmployeeCommandHandlerTests : TestBase
    {
        [Fact]
        public void Delete_should_throw_when_dbcontext_is_null()
        {
            var dbContext = (ApplicationDbContext)null;

            // Note: If DeleteEmployeeCommandHandler doesn't have an explicit null check for dbContext, this test may fail.
            // In that case, add "if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));" to its constructor.
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                new DeleteEmployeeCommandHandler(dbContext);
            });

            Assert.Equal(nameof(dbContext), exception.ParamName);
        }

        [Fact]
        public async Task Delete_should_throw_when_request_is_null()
        {
            // Arrange
            var request = (DeleteEmployeeCommand)null;
            var handler = new DeleteEmployeeCommandHandler(DbContext);

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
            var command = new DeleteEmployeeCommand { Id = id };
            var faultyDbContext = GetFaultyDbContext();

            // Depending on implementation, you may need a properly configured context here for ExecuteDeleteAsync 
            // but the test name suggests testing negative IDs
            var handler = new DeleteEmployeeCommandHandler(DbContext);

            var employee = new Employee
            {
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane.doe@example.com",
                Role = "Manager",
                Phone = "12345678" // Added specific required fields if any
            };
            await DbContext.Employees.AddAsync(employee);
            await DbContext.SaveChangesAsync();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_remove_existing_employee()
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

            var command = new DeleteEmployeeCommand { Id = employee.Id };
            var handler = new DeleteEmployeeCommandHandler(DbContext);

            // Act

            // NOTE: EF Core InMemory database DOES NOT support ExecuteDeleteAsync() out of the box in older versions. 
            // If this throws an exception 'NotSupportedException', you should consider changing the handler to use 
            // _dbContext.Employees.Remove(...) or use a SQLite in-memory database instead.
            var result = await handler.Handle(command, CancellationToken.None);

            var deletedEmployee = await DbContext.Employees.FindAsync(command.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.Null(deletedEmployee);
        }

        [Fact]
        public async Task Delete_should_not_fail_when_employee_does_not_exist()
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

            var command = new DeleteEmployeeCommand { Id = 999 };
            var handler = new DeleteEmployeeCommandHandler(DbContext);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);
            var existingEmployee = await DbContext.Employees.FindAsync(employee.Id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            Assert.NotNull(existingEmployee);
        }
    }
}
