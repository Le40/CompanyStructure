using CompanyStructure.Application.Employees.InterFaces;
using CompanyStructure.Application.Results;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Employees.Services
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

        public async Task<ServiceResult<List<EmployeeResponse>>> GetAllEmployeesAsync(int companyId)
        {
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                return ServiceResult<List<EmployeeResponse>>.Fail(ServiceErrors.CompanyNotFound);
            }

            var employees = await _db.Employees
                .Include(e => e.Company)
                .Where(e => e.CompanyId == companyId)
                .ToListAsync();

            return ServiceResult<List<EmployeeResponse>>.Ok(employees.Adapt<List<EmployeeResponse>>());
        }

        public async Task<ServiceResult<EmployeeResponse?>> GetEmployeeByIdAsync(int id)
        {
            var employee = await _db.Employees
                .Include(e => e.Company)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return ServiceResult<EmployeeResponse?>.Fail(ServiceErrors.NotFound<Employee>());
            }

            return ServiceResult<EmployeeResponse?>.Ok(employee.Adapt<EmployeeResponse>());
        }

        public async Task<ServiceResult<EmployeeResponse>> CreateEmployeeAsync(int companyId, CreateEmployeeRequest dto)
        {
            _logger.LogInformation("Creating employee for company ID {CompanyId}", companyId);
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                _logger.LogWarning("Company with ID {CompanyId} does not exist.", companyId);
                return ServiceResult<EmployeeResponse>.Fail(ServiceErrors.NotFound<Company>());
            }

            var emailExists = await _db.Employees.AnyAsync(e => e.Email == dto.Email);
            if (emailExists)
            {
                _logger.LogWarning("Email already exists.");
                return ServiceResult<EmployeeResponse>.Fail(ServiceErrors.EmailAlreadyExists);
            }

            var employee = dto.Adapt<Employee>();
            employee.CompanyId = companyId;

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {EmployeeId} created successfully.", employee.Id);
            return ServiceResult<EmployeeResponse>.Ok(employee.Adapt<EmployeeResponse>());
        }

        public async Task<ServiceResult<EmployeeResponse>> UpdateEmployeeAsync(int id, UpdateEmployeeRequest dto)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
                return ServiceResult<EmployeeResponse>.Fail(ServiceErrors.NotFound<Employee>());
            }

            var emailExists = await _db.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (emailExists)
            {
                _logger.LogWarning("Email already exists.");
                return ServiceResult<EmployeeResponse>.Fail(ServiceErrors.EmailAlreadyExists);
            }

            dto.Adapt(employee);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {EmployeeId} updated successfully.", id);
            return ServiceResult<EmployeeResponse>.Ok(employee.Adapt<EmployeeResponse>());
        }

        public async Task<ServiceResult<bool>> DeleteEmployeeAsync(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found.", id);
                return ServiceResult<bool>.Fail(ServiceErrors.NotFound<Employee>());
            }
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {EmployeeId} deleted successfully.", id);
            return ServiceResult<bool>.Ok(true);

        }
    }
}
