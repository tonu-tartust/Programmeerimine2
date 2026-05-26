using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using KooliProjekt.Application.Data;
using KooliProjekt.Application.Features.ProjectMembers;
using KooliProjekt.Application.Infrastructure.Paging;
using KooliProjekt.Application.Infrastructure.Results;
using KooliProjekt.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KooliProjekt.IntegrationTests
{
    [Collection("Sequential")]
    public class ProjectMembersControllerTests : TestBase
    {
        [Fact]
        public async Task List_should_return_paged_result()
        {
            // Arrange
            var url = "/api/ProjectMembers/List/?page=1&pageSize=10";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<PagedResult<ProjectMember>>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
        }

        [Fact]
        public async Task Get_should_return_project_member()
        {
            // Arrange
            var pm = new ProjectMember { ProjectId = 1, EmployeeId = 1, RoleInProject = "Developer" };
            await DbContext.AddAsync(pm);
            await DbContext.SaveChangesAsync();

            var url = $"/api/ProjectMembers/Get/?id={pm.Id}";

            // Act
            var response = await Client.GetFromJsonAsync<OperationResult<ProjectMember>>(url);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.HasErrors);
            Assert.NotNull(response.Value);
            Assert.Equal(pm.Id, response.Value.Id);
        }

        [Fact]
        public async Task Get_should_return_not_found_for_missing_project_member()
        {
            // Arrange
            var url = "/api/ProjectMembers/Get/?id=999";

            // Act
            var response = await Client.GetAsync(url);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_should_remove_existing_project_member()
        {
            // Arrange
            var pm = new ProjectMember { ProjectId = 1, EmployeeId = 1, RoleInProject = "Developer" };
            await DbContext.AddAsync(pm);
            await DbContext.SaveChangesAsync();

            var url = $"/api/ProjectMembers/Delete/?id={pm.Id}";

            // Act
            using var response = await Client.DeleteAsync(url);
            
            var itemFromDb = await DbContext.ProjectMembers
                .Where(x => x.Id == pm.Id)
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Delete_should_work_with_missing_project_member()
        {
            // Arrange
            var url = "/api/ProjectMembers/Delete/?id=101";

            // Act
            using var response = await Client.DeleteAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_add_new_project_member()
        {
            // Arrange
            var url = "/api/ProjectMembers/Save/";
            var command = new SaveProjectMembersCommand { ProjectId = 1, EmployeeId = 1, RoleInProject = "Developer" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.ProjectMembers
                .Where(x => x.RoleInProject == "Developer" && x.ProjectId == 1)
                .FirstOrDefaultAsync();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.False(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_missing_project_member()
        {
            // Arrange
            var url = "/api/ProjectMembers/Save/";
            var command = new SaveProjectMembersCommand { Id = 100, ProjectId = 1, EmployeeId = 1, RoleInProject = "Developer" };

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);
            var itemFromDb = await DbContext.ProjectMembers
                .Where(x => x.Id == 100)
                .FirstOrDefaultAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Null(itemFromDb);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }

        [Fact]
        public async Task Save_should_work_with_invalid_project_member()
        {
            // Arrange
            var url = "/api/ProjectMembers/Save/";
            var command = new SaveProjectMembersCommand { Id = 0, RoleInProject = "" }; // Invalid

            // Act
            using var response = await Client.PostAsJsonAsync(url, command);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var result = await response.Content.ReadFromJsonAsync<OperationResult>();
            Assert.True(result.HasErrors);
        }
    }
}