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
    public class ProjectMember
    {
        [Key]
        [Required]
        [Unicode]
        public int Id { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public string RoleInProject { get; set; }
    }
}
