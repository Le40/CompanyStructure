using System.ComponentModel.DataAnnotations;

namespace CompanyStructure.Application.Nodes
{
    public class CreateNodeRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(30)]
        public required string Code { get; set; }

        public int? LeaderId { get; set; } = null;
    }
}
