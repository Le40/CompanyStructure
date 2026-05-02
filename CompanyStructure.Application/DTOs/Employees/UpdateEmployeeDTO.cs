using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CompanyStructure.Application.DTOs.Employees
{
    public class UpdateEmployeeDTO
    {
        [MaxLength(25)]
        public string? Degree { get; set; }
        [MaxLength(50)]
        public string? Name { get; set; }
        [MaxLength(100)]
        public string? Surname { get; set; }
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [MaxLength(50)]
        public string? Email { get; set; }
        [Phone(ErrorMessage = "Phone format is invalid")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

    }
}
