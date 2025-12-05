using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data.Repositories
{
    // ProjectMember repository interface
    public interface IProjectMemberRepository
    {
        Task<ProjectMember> GetByIdAsync(int id);
        Task SaveAsync(ProjectMember entity);
        Task DeleteAsync(ProjectMember entity);
    }
}
