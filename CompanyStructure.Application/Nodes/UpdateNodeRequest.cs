using System.ComponentModel.DataAnnotations;

namespace CompanyStructure.Application.Nodes
{
    public class UpdateNodeRequest
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(30)]
        public required string Code { get; set; }

        public int? LeaderId { get; set; } = null;
    }
}
