using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyStructure.Application.DTOs.Employees
{
    public class UpdateEmployeeDTO
    {
        /*[MaxLength(25)]
        public string? Degree { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(100)]
        public string Surname { get; set; }
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [MaxLength(50)]
        public string Email { get; set; }
        [Phone(ErrorMessage = "Phone format is invalid")]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }*/
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
