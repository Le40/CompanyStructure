namespace CompanyStructure.Models
{
    public class Department : IOrganizationNode
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public int? LeaderId { get; set; }
        public Employee? Leader { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
