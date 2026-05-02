namespace CompanyStructure.Models
{
    public class Company : IOrganizationNode
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Code { get; set; }
        public int? LeaderId { get; set; }
        public Employee? Leader { get; set; }

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public ICollection<Division> Divisions { get; set; } = new List<Division>();
    }
}
