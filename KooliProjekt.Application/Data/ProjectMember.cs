using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KooliProjekt.Application.Data
{
    public class ProjectMember : Entity
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string RoleInProject { get; set; }
    }
}
