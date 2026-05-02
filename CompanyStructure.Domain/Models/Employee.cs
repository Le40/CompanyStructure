using System.ComponentModel.DataAnnotations;

namespace CompanyStructure.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public int CompanyId { get; set; }
        public Company? Company { get; set; } = null;

        [MaxLength(25)]
        public string? Degree { get; set; }
        [MaxLength(50)]
        public required string Name { get; set; }
        [MaxLength(100)]
        public required string Surname { get; set; }
        [MaxLength(50)]
        public required string Email { get; set; }
        [MaxLength(20)]
        public required string PhoneNumber { get; set; }
    }
}
