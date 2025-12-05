using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Data.Repositories
{
    // Task repository klass
    public class TaskRepository : BaseRepository<Tasks>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext dbContext) : 
            base(dbContext)
        {
        }

        // Lisa siia spetsiifilisemad meetodid,
        // mis on seotud Taskidega
    }
}
