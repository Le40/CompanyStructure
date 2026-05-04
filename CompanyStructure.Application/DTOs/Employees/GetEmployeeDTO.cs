using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.DTOs.Employees
{
    public class GetEmployeeDTO
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = null!;

        public string? Degree{ get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
