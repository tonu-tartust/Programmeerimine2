using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.Projects;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    [Collection("Sequential")]
    public class ProjectsControllerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_result()
        {
            // Arrange
            var url = "/api/Projects/List/?page=1&pageSize=10";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<PagedResult<Project>>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
        }

        [Fact]
        public async Task Get_should_return_project()
        {
            // Arrange
            var project = new Project { Name = "Test Project", Description = "Desc", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10), Status = "Active", Budget = 1000 };
            await DbContext.AddAsync(project);
            await DbContext.SaveChangesAsync();

            var url = $"/api/Projects/Get/?id={project.Id}";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<Project>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
            Assert.NotNull(response.Value);
            Assert.Equal(project.Id, response.Value.Id);
        }

        [Fact]
        public async Task Get_should_return_not_found_for_missing_project()
        {
            // Arrange
            var url = "/api/Projects/Get/?id=999";

            // Act
            var response = await Client.GetAsync(url);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_should_remove_existing_project()
        {
            // Arrange
            var project = new Project { Name = "Test Project", Description = "Desc", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10), Status = "Active", Budget = 1000 };
            await DbContext.AddAsync(project);
            await DbContext.SaveChangesAsync();

            var url = $"/api/Projects/Delete/?id={project.Id}";

            // Act
            using var response = await Client.DeleteAsync(url);

            var itemFromDb = await DbContext.Projects
                .Where(x => x.Id == project.Id)
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_work_with_missing_project()
        {
            // Arrange
            var url = "/api/Projects/Delete/?id=101";

            // Act
            using var response = await Client.DeleteAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_add_new_project()
        {
            // Arrange
            var url = "/api/Projects/Save/";
            var command = new SaveProjectsCommand { Name = "Test Project", Description = "Desc", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10), Status = "Active", Budget = 1000 };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.Projects
                .Where(x => x.Name == "Test Project")
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_missing_project()
        {
            // Arrange
            var url = "/api/Projects/Save/";
            var command = new SaveProjectsCommand { Id = 100, Name = "Test Project", Description = "Desc", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10), Status = "Active", Budget = 1000 };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.Projects
                .Where(x => x.Id == 100)
                .FirstOrDefaultAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_invalid_project()
        {
            // Arrange
            var url = "/api/Projects/Save/";
            var command = new SaveProjectsCommand { Id = 0, Name = "" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }
    }
}