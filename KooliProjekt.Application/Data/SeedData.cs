using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data
{
    // SeedData klass andmete genereerimiseks
    public class SeedData
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IList<Project> _projects = new List<Project>();
        private readonly IList<Employee> _employees = new List<Employee>();

        public SeedData(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        // Generate meetod koordineerib andmete genereerimist
        public void Generate()
        {
            // Ära tee midagi kui andmed on juba olemas
            if (_dbContext.Projects.Any())
            {
                return;
            }

            GenerateEmployees();
            GenerateProjects();
            GenerateProjectMembers();
            GenerateTasks();

            _dbContext.SaveChanges();
        }

        private void GenerateEmployees()
        {
            var roles = new[] { "Developer", "Designer", "Manager", "Analyst", "Tester" };
            
            for (var i = 0; i < 10; i++)
            {
                var employee = new Employee
                {
                    FirstName = $"FirstName{i + 1}",
                    LastName = $"LastName{i + 1}",
                    Email = $"employee{i + 1}@example.com",
                    Phone = $"+372 5{i}00 000{i}",
                    Role = roles[i % roles.Length]
                };

                _employees.Add(employee);
            }

            _dbContext.Employees.AddRange(_employees);
        }

        private void GenerateProjects()
        {
            var statuses = new[] { "Planning", "In Progress", "Completed", "On Hold" };
            
            for (var i = 0; i < 5; i++)
            {
                var project = new Project
                {
                    Name = $"Project {i + 1}",
                    Description = $"Description for Project {i + 1}",
                    StartDate = DateTime.Now.AddDays(-30 + i * 10),
                    EndDate = DateTime.Now.AddDays(60 + i * 10),
                    Status = statuses[i % statuses.Length],
                    Budget = 10000 + (i * 5000)
                };

                _projects.Add(project);
            }

            _dbContext.Projects.AddRange(_projects);
        }

        private void GenerateProjectMembers()
        {
            var roles = new[] { "Lead", "Member", "Consultant" };
            var memberIndex = 0;

            foreach (var project in _projects)
            {
                for (var j = 0; j < 3; j++)
                {
                    var employee = _employees[memberIndex % _employees.Count];
                    
                    var projectMember = new ProjectMember
                    {
                        ProjectId = project.Id,
                        EmployeeId = employee.Id,
                        RoleInProject = roles[j % roles.Length]
                    };

                    _dbContext.ProjectMembers.Add(projectMember);
                    memberIndex++;
                }
            }
        }

        private void GenerateTasks()
        {
            var statuses = new[] { "Not Started", "In Progress", "Completed", "Blocked" };
            var priorities = new[] { "Low", "Medium", "High", "Critical" };
            var taskIndex = 0;

            foreach (var project in _projects)
            {
                for (var j = 0; j < 4; j++)
                {
                    var employee = _employees[taskIndex % _employees.Count];
                    
                    var task = new Tasks
                    {
                        ProjectId = project.Id,
                        Title = $"Task {j + 1} for {project.Name}",
                        Description = $"Description for task {j + 1}",
                        AssignedTo = employee.Id,
                        StartDate = project.StartDate.AddDays(j * 7),
                        DueDate = project.StartDate.AddDays((j + 1) * 7),
                        Status = statuses[j % statuses.Length],
                        Priority = priorities[j % priorities.Length]
                    };

                    _dbContext.Taskss.Add(task);
                    taskIndex++;
                }
            }
        }
    }
}
