using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data.Repositories
{
    // Project repository interface
    public interface IProjectRepository
    {
        Task<Project> GetByIdAsync(int id);
        Task SaveAsync(Project entity);
        Task DeleteAsync(Project entity);
    }
}
