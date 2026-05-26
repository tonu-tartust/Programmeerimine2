using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Task;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    [Collection("Sequential")]
    public class TasksControllerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_result()
        {
            // Arrange
            var url = "/api/Tasks/List/?page=1&pageSize=10";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<PagedResult<KooliProjekt.Application.Data.Tasks>>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
        }

        [Fact]
        public async Task Get_should_return_task()
        {
            // Arrange
            var task = new KooliProjekt.Application.Data.Tasks { ProjectId = 1, Title = "Test Task", Description = "Desc", AssignedTo = 1, StartDate = DateTime.Now, DueDate = DateTime.Now.AddDays(5), Status = "Open", Priority = "High" };
            await DbContext.AddAsync(task);
            await DbContext.SaveChangesAsync();

            var url = $"/api/Tasks/Get/?id={task.Id}";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<KooliProjekt.Application.Data.Tasks>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
            Assert.NotNull(response.Value);
            Assert.Equal(task.Id, response.Value.Id);
        }

        [Fact]
        public async Task Get_should_return_not_found_for_missing_task()
        {
            // Arrange
            var url = "/api/Tasks/Get/?id=999";

            // Act
            var response = await Client.GetAsync(url);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_should_remove_existing_task()
        {
            // Arrange
            var task = new KooliProjekt.Application.Data.Tasks { ProjectId = 1, Title = "Test Task", Description = "Desc", AssignedTo = 1, StartDate = DateTime.Now, DueDate = DateTime.Now.AddDays(5), Status = "Open", Priority = "High" };
            await DbContext.AddAsync(task);
            await DbContext.SaveChangesAsync();

            var url = $"/api/Tasks/Delete/?id={task.Id}";

            // Act
            using var response = await Client.DeleteAsync(url);
            
            var itemFromDb = await DbContext.Taskss
                .Where(x => x.Id == task.Id)
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_work_with_missing_task()
        {
            // Arrange
            var url = "/api/Tasks/Delete/?id=101";

            // Act
            using var response = await Client.DeleteAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_add_new_task()
        {
            // Arrange
            var url = "/api/Tasks/Save/";
            var command = new SaveTasksCommand { ProjectId = 1, Title = "Test Task", Description = "Desc", AssignedTo = 1, StartDate = DateTime.Now, DueDate = DateTime.Now.AddDays(5), Status = "Open", Priority = "High" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.Taskss
                .Where(x => x.Title == "Test Task")
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_missing_task()
        {
            // Arrange
            var url = "/api/Tasks/Save/";
            var command = new SaveTasksCommand { Id = 100, ProjectId = 1, Title = "Test Task", Description = "Desc", AssignedTo = 1, StartDate = DateTime.Now, DueDate = DateTime.Now.AddDays(5), Status = "Open", Priority = "High" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.Taskss
                .Where(x => x.Id == 100)
                .FirstOrDefaultAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_invalid_task()
        {
            // Arrange
            var url = "/api/Tasks/Save/";
            var command = new SaveTasksCommand { Id = 0, Title = "" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }
    }
}