using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace KooliProjekt.Application.Data.Repositories
{
    // Employee repository klass
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : 
            base(dbContext)
        {
        }

        // Lisa siia spetsiifilisemad meetodid,
        // mis on seotud Employeedega
    }
}
