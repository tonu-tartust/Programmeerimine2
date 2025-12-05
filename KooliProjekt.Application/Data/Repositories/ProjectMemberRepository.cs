using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Data.Repositories
{
    // ProjectMember repository klass
    public class ProjectMemberRepository : BaseRepository<ProjectMember>, IProjectMemberRepository
    {
        public ProjectMemberRepository(ApplicationDbContext dbContext) : 
            base(dbContext)
        {
        }

        // Lisa siia spetsiifilisemad meetodid,
        // mis on seotud ProjectMemberitega
    }
}
