using System.ComponentModel.DataAnnotations;

namespace CompanyStructure.Domain.Models
{
    public class Division : IOrganisationNode
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        [MaxLength(50)]
        public required string Code { get; set; }
        public int? LeaderId { get; set; }
        public Employee? Leader { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
