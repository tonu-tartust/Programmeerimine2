namespace KooliProjekt.Application.DTO
{
    public class ProjectMemberDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string RoleInProject { get; set; }
    }
}
