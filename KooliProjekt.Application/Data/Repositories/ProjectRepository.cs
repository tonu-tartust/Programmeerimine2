using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Data.Repositories
{
    // Project repository klass
    public class ProjectRepository : BaseRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext dbContext) : 
            base(dbContext)
        {
        }

        // Lisa siia spetsiifilisemad meetodid,
        // mis on seotud Projectidega
    }
}
