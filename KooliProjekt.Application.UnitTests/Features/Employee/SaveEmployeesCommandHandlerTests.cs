using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Data.Repositories;
using KooliProjekt.Application.Features.Employees;
using Moq;
using Xunit;

namespace KooliProjekt.Application.UnitTests.Features.Employees
{
    public class SaveEmployeesCommandHandlerTests : TestBase
    {
        [Fact]
        public void Save_should_throw_when_repository_is_null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new SaveEmployeesCommandHandler(null);
            });
        }

        [Fact]
        public async Task Save_should_throw_when_request_is_null()
        {
             var mockRepository = new Mock<IEmployeeRepository>();
             var handler = new SaveEmployeesCommandHandler(mockRepository.Object);
             await Assert.ThrowsAsync<ArgumentNullException>(async () =>
             {
                 await handler.Handle(null, CancellationToken.None);
             });
        }

        [Fact]
        public async Task Save_should_return_error_when_existing_employee_is_not_found()
        {
            var mockRepository = new Mock<IEmployeeRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee)null);

            var request = new SaveEmployeesCommand { Id = 1 };
            var handler = new SaveEmployeesCommandHandler(mockRepository.Object);

            var result = await handler.Handle(request, CancellationToken.None);

            Assert.NotNull(result);
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_save_new_employee()
        {
            // Arrange
            var mockRepository = new Mock<IEmployeeRepository>();
            Employee savedEmployee = null;
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<Employee>()))
                .Callback<Employee>(e => savedEmployee = e)
                .Returns(Task.CompletedTask);

            var request = new SaveEmployeesCommand
            {
                Id = 0,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Role = "Developer"
            };
            var handler = new SaveEmployeesCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<Employee>()), Times.Once);
            Assert.NotNull(savedEmployee);
            Assert.Equal(request.FirstName, savedEmployee.FirstName);
            Assert.Equal(request.LastName, savedEmployee.LastName);
            Assert.Equal(request.Email, savedEmployee.Email);
            Assert.Equal(request.Phone, savedEmployee.Phone);
            Assert.Equal(request.Role, savedEmployee.Role);
        }

        [Fact]
        public async Task Save_should_update_existing_employee()
        {
            // Arrange
            var existingEmployee = new Employee
            {
                Id = 1,
                FirstName = "Old",
                LastName = "Name",
                Email = "old@example.com",
                Phone = "0000000000",
                Role = "Intern"
            };

            var mockRepository = new Mock<IEmployeeRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingEmployee);
            mockRepository.Setup(r => r.SaveAsync(It.IsAny<Employee>()))
                .Returns(Task.CompletedTask);

            var request = new SaveEmployeesCommand
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Role = "Developer"
            };
            var handler = new SaveEmployeesCommandHandler(mockRepository.Object);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.HasErrors);
            mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
            mockRepository.Verify(r => r.SaveAsync(It.IsAny<Employee>()), Times.Once);
            Assert.Equal(request.FirstName, existingEmployee.FirstName);
            Assert.Equal(request.LastName, existingEmployee.LastName);
            Assert.Equal(request.Email, existingEmployee.Email);
            Assert.Equal(request.Phone, existingEmployee.Phone);
            Assert.Equal(request.Role, existingEmployee.Role);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_FirstName_is_invalid(string firstName)
        {
            // Arrange
            var validator = new SaveEmployeesCommandValidator(DbContext);
            var command = new SaveEmployeesCommand
            {
                Id = 0,
                FirstName = firstName,
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Role = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveEmployeesCommand.FirstName), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_LastName_is_invalid(string lastName)
        {
            // Arrange
            var validator = new SaveEmployeesCommandValidator(DbContext);
            var command = new SaveEmployeesCommand
            {
                Id = 0,
                FirstName = "John",
                LastName = lastName,
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Role = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveEmployeesCommand.LastName), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-email")]
        public void SaveValidator_should_return_false_when_Email_is_invalid(string email)
        {
            // Arrange
            var validator = new SaveEmployeesCommandValidator(DbContext);
            var command = new SaveEmployeesCommand
            {
                Id = 0,
                FirstName = "John",
                LastName = "Doe",
                Email = email,
                Phone = "1234567890",
                Role = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveEmployeesCommand.Email), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Phone_is_invalid(string phone)
        {
            // Arrange
            var validator = new SaveEmployeesCommandValidator(DbContext);
            var command = new SaveEmployeesCommand
            {
                Id = 0,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = phone,
                Role = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveEmployeesCommand.Phone), result.Errors.First().PropertyName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void SaveValidator_should_return_false_when_Role_is_invalid(string role)
        {
            // Arrange
            var validator = new SaveEmployeesCommandValidator(DbContext);
            var command = new SaveEmployeesCommand
            {
                Id = 0,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Role = role
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(nameof(SaveEmployeesCommand.Role), result.Errors.First().PropertyName);
        }

        [Fact]
        public void SaveValidator_should_return_true_when_data_is_valid()
        {
            // Arrange
            var validator = new SaveEmployeesCommandValidator(DbContext);
            var command = new SaveEmployeesCommand
            {
                Id = 0,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "1234567890",
                Role = "Developer"
            };

            // Act
            var result = validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
