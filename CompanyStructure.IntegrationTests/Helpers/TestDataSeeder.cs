using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;

namespace CompanyStructure.IntegrationTests.Helpers
{
    public class TestDataSeeder
    {
        private readonly AppDbContext _db;

        public TestDataSeeder(AppDbContext db)
        {
            _db = db;
        }

        public record SeedCompany(int Id, string Code);
        public record SeedNode(int Id, int CompanyId, string Code);
        public record SeedEmployee(int Id, int CompanyId, string Email);

        public async Task<SeedCompany> SeedCompanyAsync(
            string? name = null,
            string? code = null,
            CancellationToken cancellationToken = default)
        {
            var unique = Guid.NewGuid().ToString("N")[..8];

            var company = new Company
            {
                Name = name ?? $"Company {unique}",
                Code = code ?? $"C-{unique}"
            };

            _db.Companies.Add(company);
            await _db.SaveChangesAsync(cancellationToken);

            return new SeedCompany(company.Id, company.Code);
        }

        public async Task<SeedEmployee> SeedEmployeeAsync(int? companyId, string? email = null,
            CancellationToken cancellationToken = default)
        {
            var seed = await SeedCompanyAsync(); 

            var unique = Guid.NewGuid().ToString("N")[..8];

            var employee = new Employee
            {
                Name = "John",
                Surname = "Doe",
                Email = email ?? $"john.doe.{unique}@test.com",
                PhoneNumber = "+421900123456",
                CompanyId = seed.Id
            };

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync(cancellationToken);

            return new SeedEmployee(employee.Id, seed.Id, employee.Email);
        }

        public async Task<SeedNode> SeedDivisionAsync(
            string? name = null,
            string? code = null,
             int? leaderId = null,
            CancellationToken cancellationToken = default)
        {
            var seed = await SeedCompanyAsync();

            var unique = Guid.NewGuid().ToString("N")[..8];

            var division = new Division
            {
                Name = name ?? $"IT Division {unique}",
                Code = code ?? $"DIV-{unique}",
                LeaderId = leaderId,
                CompanyId = seed.Id
            };
  

            _db.Divisions.Add(division);
            await _db.SaveChangesAsync(cancellationToken);

            return new SeedNode(division.Id, division.CompanyId, division.Code);
        }

        public async Task<SeedNode> SeedProjectAsync(
            string? name = null,
            string? code = null,
             int? leaderId = null,
            CancellationToken cancellationToken = default)
        {
            var seed = await SeedDivisionAsync();

            var unique = Guid.NewGuid().ToString("N")[..8];

            var project = new Project
            {
                Name = name ?? $"ITP Project {unique}",
                Code = code ?? $"PRJ-{unique}",
                LeaderId = leaderId,
                CompanyId = seed.CompanyId,
                DivisionId = seed.Id
            };

            _db.Projects.Add(project);
            await _db.SaveChangesAsync(cancellationToken);

            return new SeedNode(project.Id, project.CompanyId, project.Code);
        }

        public async Task<SeedNode> SeedDepartmentAsync(
            string? name = null,
            string? code = null,
             int? leaderId = null,
            CancellationToken cancellationToken = default)
        {
            var seed = await SeedProjectAsync();

            var unique = Guid.NewGuid().ToString("N")[..8];

            var department = new Department
            {
                Name = name ?? $"ITP Project {unique}",
                Code = code ?? $"PRJ-{unique}",
                LeaderId = leaderId,
                CompanyId = seed.CompanyId,
                ProjectId = seed.Id
            };

            _db.Departments.Add(department);
            await _db.SaveChangesAsync(cancellationToken);

            return new SeedNode(department.Id, department.CompanyId, department.Code);
        }
    }
}
