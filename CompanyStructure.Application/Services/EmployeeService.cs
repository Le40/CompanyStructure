using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CompanyStructure.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;

        public EmployeeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync(int? companyID)
            => await _db.Employees.ToListAsync();

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _db.Employees.FindAsync(id);
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            throw new NotImplementedException();    
        }

        public Task<Employee> UpdateEmployeeAsync(int id, Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<Employee> DeleteEmployeeAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
