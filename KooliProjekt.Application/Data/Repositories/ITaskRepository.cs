using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data.Repositories
{
    // Task repository interface
    public interface ITaskRepository
    {
        Task<Tasks> GetByIdAsync(int id);
        Task SaveAsync(Tasks entity);
        Task DeleteAsync(Tasks entity);
    }
}
