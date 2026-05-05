using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyStructure.Application.DTOs.OrganisationNodes
{
    public class UpdateOrganisationNodeDTO
    {
        [MaxLength(100)]
        public required string Name { get; set; }

        [MaxLength(30)]
        public required string Code { get; set; }

        public int? LeaderId { get; set; } = null;
    }
}
