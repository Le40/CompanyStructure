using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyStructure.Application.DTOs.OrganisationNodes
{
    public class UpdateOrganisationNodeDTO
    {
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(30)]
        public string? Code { get; set; }

        public int? LeaderId { get; set; }
    }
}
