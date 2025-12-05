using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data.Repositories
{
    // Employee repository interface
    public interface IEmployeeRepository
    {
        Task<Employee> GetByIdAsync(int id);
        Task SaveAsync(Employee entity);
        Task DeleteAsync(Employee entity);
    }
}
