using System;
using FluentValidation;
using KooliProjekt.Application.Behaviors;
using KooliProjekt.Application.Data;
using KooliProjekt.WebAPI.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace KooliProjekt.IntegrationTests.Helpers
{
    public class FakeStartup
    {
        public FakeStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            var dbGuid = Guid.NewGuid().ToString();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbGuid);
            });

            var applicationAssembly = typeof(ErrorHandlingBehavior<,>).Assembly;
            services.AddValidatorsFromAssembly(applicationAssembly);
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(applicationAssembly);
                config.AddOpenBehavior(typeof(ErrorHandlingBehavior<,>));
                // config.AddOpenBehavior(typeof(TransactionalBehavior<,>)); // Ignored for InMemory Db
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddControllersWithViews()
                    .AddApplicationPart(typeof(ApiControllerBase).Assembly);

            services.AddScoped<KooliProjekt.Application.Data.Repositories.IEmployeeRepository, KooliProjekt.Application.Data.Repositories.EmployeeRepository>();
            services.AddScoped<KooliProjekt.Application.Data.Repositories.IProjectRepository, KooliProjekt.Application.Data.Repositories.ProjectRepository>();
            services.AddScoped<KooliProjekt.Application.Data.Repositories.IProjectMemberRepository, KooliProjekt.Application.Data.Repositories.ProjectMemberRepository>();
            services.AddScoped<KooliProjekt.Application.Data.Repositories.ITaskRepository, KooliProjekt.Application.Data.Repositories.TaskRepository>();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}/{pathStr?}");
            });

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                if (dbContext == null)
                {
                    throw new NullReferenceException("Cannot get instance of dbContext");
                }

                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}