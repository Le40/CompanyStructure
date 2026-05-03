using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyStructure.Application.DTOs.OrganisationNodes
{
    public class CreateOrganisationNodeDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100)]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(30)]
        public string? Code { get; set; }

        public int? LeaderId { get; set; }
    }
}
