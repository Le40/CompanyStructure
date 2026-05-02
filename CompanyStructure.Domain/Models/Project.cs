using System.ComponentModel.DataAnnotations;

namespace CompanyStructure.Domain.Models
{
    public class Project : IOrganizationNode
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public required string Code { get; set; }
        public int? LeaderId { get; set; }
        public Employee? Leader { get; set; }

        public int DivisionId { get; set; }
        public Division? Division { get; set; }
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }
}
