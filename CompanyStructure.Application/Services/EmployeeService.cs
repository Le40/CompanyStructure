using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<Employee> _logger;

        public EmployeeService(AppDbContext db, ILogger<Employee> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<ServiceResult<List<GetEmployeeDTO>>> GetAllEmployeesAsync(int companyId)
        {
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                return ServiceResult<List<GetEmployeeDTO>>.Fail("Company does not exist.", ServiceErrorType.NotFound);
            }

            var employees = await _db.Employees
                .Include(e => e.Company)
                .Where(e => e.CompanyId == companyId)
                .ToListAsync();

            return ServiceResult<List<GetEmployeeDTO>>.Ok(employees.Adapt<List<GetEmployeeDTO>>());
        }

        public async Task<ServiceResult<GetEmployeeDTO?>> GetEmployeeByIdAsync(int id)
        {
            var employee = await _db.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return ServiceResult<GetEmployeeDTO?>.Fail("Employee not found.", ServiceErrorType.NotFound);
            }

            return ServiceResult<GetEmployeeDTO?>.Ok(employee.Adapt<GetEmployeeDTO>());
        }

        public async Task<ServiceResult<GetEmployeeDTO>> CreateEmployeeAsync(int companyId, CreateEmployeeDTO dto)
        {
            _logger.LogInformation("Creating employee for company ID {CompanyId}", companyId);
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                _logger.LogWarning("Company with ID {CompanyId} does not exist.", companyId);
                return ServiceResult<GetEmployeeDTO>.Fail("Company does not exist.", ServiceErrorType.NotFound);
            }

            var emailExists = await _db.Employees.AnyAsync(e => e.Email == dto.Email);
            if (emailExists)
            {
                _logger.LogWarning("Email already exists.");
                return ServiceResult<GetEmployeeDTO>.Fail("Email already exists.", ServiceErrorType.Conflict);
            }

            var employee = dto.Adapt<Employee>();
            employee.CompanyId = companyId;

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {EmployeeId} created successfully.", employee.Id);
            return ServiceResult<GetEmployeeDTO>.Ok(employee.Adapt<GetEmployeeDTO>());
        }

        public async Task<ServiceResult<GetEmployeeDTO>> UpdateEmployeeAsync(int id, UpdateEmployeeDTO dto)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
                return ServiceResult<GetEmployeeDTO>.Fail("Employee not found.", ServiceErrorType.NotFound);
            }

            var emailExists = await _db.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (emailExists)
            {
                _logger.LogWarning("Email already exists.");
                return ServiceResult<GetEmployeeDTO>.Fail("Email already exists.", ServiceErrorType.Conflict);
            }

            dto.Adapt(employee);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {EmployeeId} updated successfully.", id);
            return ServiceResult<GetEmployeeDTO>.Ok(employee.Adapt<GetEmployeeDTO>());
        }

        public async Task<ServiceResult<bool>> DeleteEmployeeAsync(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
                return ServiceResult<bool>.Fail("Employee not found.", ServiceErrorType.NotFound);
            }
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {EmployeeId} deleted successfully.", id);
            return ServiceResult<bool>.Ok(true);

        }
    }
}
