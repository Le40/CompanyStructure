using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyStructure.Application.DTOs.Employees
{
    public class CreateEmployeeDTO
    {
        public int CompanyId { get; set; }

        [MaxLength(25)]
        public string? Degree { get; set; }
        [Required]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Surname { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(50)]
        public required string Email { get; set; }
        [Required]
        [Phone]
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
    }
}
