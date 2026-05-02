namespace CompanyStructure.Models
{
    public class Division : IOrganizationNode
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public int? LeaderId { get; set; }
        public Employee? Leader { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
