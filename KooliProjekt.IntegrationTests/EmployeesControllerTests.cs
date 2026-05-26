using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Employees;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    [Collection("Sequential")]
    public class EmployeesControllerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_result()
        {
            // Arrange
            var url = "/api/Employees/List/?page=1&pageSize=10";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<PagedResult<Employee>>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
        }

        [Fact]
        public async Task Get_should_return_employee()
        {
            // Arrange
            var employee = new Employee { FirstName = "Jane", LastName = "Doe", Email = "j@example.com", Phone = "1234", Role = "Tester" };
            await DbContext.AddAsync(employee);
            await DbContext.SaveChangesAsync();

            var url = $"/api/Employees/Get/?id={employee.Id}";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<Employee>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
            Assert.NotNull(response.Value);
            Assert.Equal(employee.Id, response.Value.Id);
        }

        [Fact]
        public async Task Get_should_return_not_found_for_missing_employee()
        {
            // Arrange
            var url = "/api/Employees/Get/?id=999";

            // Act
            var response = await Client.GetAsync(url);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_should_remove_existing_employee()
        {
            // Arrange
            var employee = new Employee { FirstName = "Jane", LastName = "Doe", Email = "j@example.com", Phone = "1234", Role = "Tester" };
            await DbContext.AddAsync(employee);
            await DbContext.SaveChangesAsync();

            var url = $"/api/Employees/Delete/?id={employee.Id}";

            // Act
            using var response = await Client.DeleteAsync(url);

            var itemFromDb = await DbContext.Employees
                .Where(x => x.Id == employee.Id)
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_work_with_missing_employee()
        {
            // Arrange
            var url = "/api/Employees/Delete/?id=101";

            // Act
            using var response = await Client.DeleteAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_add_new_employee()
        {
            // Arrange
            var url = "/api/Employees/Save/";
            var command = new SaveEmployeesCommand { FirstName = "New", LastName = "Employee", Email = "new@example.com", Phone = "1234", Role = "Tester" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.Employees
                .Where(x => x.Email == "new@example.com")
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_missing_employee()
        {
            // Arrange
            var url = "/api/Employees/Save/";
            var command = new SaveEmployeesCommand { Id = 100, FirstName = "New", LastName = "Employee", Email = "missing@example.com", Phone = "1234", Role = "Tester" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.Employees
                .Where(x => x.Id == 100)
                .FirstOrDefaultAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_invalid_employee()
        {
            // Arrange
            var url = "/api/Employees/Save/";
            var command = new SaveEmployeesCommand { Id = 0, FirstName = "" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }
    }
}