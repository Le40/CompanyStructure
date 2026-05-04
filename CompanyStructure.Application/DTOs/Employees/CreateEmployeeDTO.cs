using CompanyStructure.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyStructure.Application.DTOs.Employees
{
    public class CreateEmployeeDTO
    {
        //[Required(ErrorMessage = "Company is required.")]
        //public int CompanyId { get; set; }
        [MaxLength(25)]
        public string? Degree { get; set; }
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(100)]
        public required string Surname { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [MaxLength(50)]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Phone format is invalid")]
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
    }
}
