using System.ComponentModel.DataAnnotations;

namespace CompanyStructure.Domain.Models
{
    public class Department : IOrganisationNode
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public required string Code { get; set; }
        public required int CompanyId { get; set; }
        public int? LeaderId { get; set; }
        public Employee? Leader { get; set; }

        public int ProjectId { get; set; }
        public Project? Project { get; set; }
    }
}
