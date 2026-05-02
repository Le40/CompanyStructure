using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
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

        public async Task<List<GetEmployeeDTO>> GetAllEmployeesAsync(int? companyID)
        {
            var query = _db.Employees
                .Include(e => e.Company)
                .AsQueryable();

            if (companyID.HasValue)
            {
                query = query.Where(e => e.CompanyId == companyID.Value);
            }
            var employees = await query.ToListAsync();
            return employees.Adapt<List<GetEmployeeDTO>>();
        }


        public async Task<GetEmployeeDTO?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _db.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
                return null;

            return employee.Adapt<GetEmployeeDTO>();
        }

        public async Task<ServiceResult<GetEmployeeDTO>> CreateEmployeeAsync(CreateEmployeeDTO dto)
        {
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == dto.CompanyId);
            if (!companyExists)
            {
                return ServiceResult<GetEmployeeDTO>.Fail("Company does not exist.");
            }

            var employee = dto.Adapt<Employee>();

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();
            return ServiceResult<GetEmployeeDTO>.Ok(employee.Adapt<GetEmployeeDTO>());
        }

        public async Task<ServiceResult<GetEmployeeDTO>> UpdateEmployeeAsync(int id, UpdateEmployeeDTO dto)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return ServiceResult<GetEmployeeDTO>.Fail("Employee not found.");
            }

            employee = dto.Adapt(employee);

            _db.Employees.Update(employee);
            await _db.SaveChangesAsync();
            return ServiceResult<GetEmployeeDTO>.Ok(employee.Adapt<GetEmployeeDTO>());
        }

        public async Task<ServiceResult<bool>> DeleteEmployeeAsync(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return ServiceResult<bool>.Fail("Employee not found.");
            }
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);

        }
    }
}
