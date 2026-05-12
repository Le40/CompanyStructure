using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Common.Extensions;
using CompanyStructure.Application.Employees.InterFaces;
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

        public async Task<ServiceResult<PagedResult<EmployeeResponse>>> GetAllEmployeesAsync(int companyId, PaginationQuery pagination)
        {
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                return ServiceResult<PagedResult<EmployeeResponse>>.Fail(ServiceErrors.CompanyNotFound);
            }

            var employees = await _db.Employees
                .Include(e => e.Company)
                .Where(e => e.CompanyId == companyId)
                .ToPagedResultAsync<Employee, EmployeeResponse>(pagination.Page, pagination.PageSize);

            return ServiceResult<PagedResult<EmployeeResponse>>.Ok(employees);
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

            var emailValidation = await ValidateEmailIsUniqueAsync(dto.Email);
            if (!emailValidation.Success)
            {
                return ServiceResult<EmployeeResponse>.Fail(emailValidation.Error!);
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

            var emailValidation = await ValidateEmailIsUniqueAsync(dto.Email, id);
            if (!emailValidation.Success)
            {
                return ServiceResult<EmployeeResponse>.Fail(emailValidation.Error!);
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

            // Check if the employee is a leader of any node
            await ClearLeaderReferencesAsync(id);

            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Employee with ID {EmployeeId} deleted successfully.", id);
            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidateEmailIsUniqueAsync(string email, int? excludeId = null)
        {
            var emailExists = await _db.Employees
                .AnyAsync(e => e.Email == email && (!excludeId.HasValue || e.Id != excludeId.Value));

            if (emailExists)
            {
                _logger.LogWarning("Email already exists.");
                return ServiceResult<bool>.Fail(ServiceErrors.EmailAlreadyExists);
            }
            return ServiceResult<bool>.Ok(true);
        }

        private async Task ClearLeaderReferencesAsync(int employeeId)
        {
            // DB didnt allow to set leaderId to null when deleting employee. So all leader references needs to be cleared before deleting employee.
            await _db.Companies
                .Where(c => c.LeaderId == employeeId)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.LeaderId, (int?)null));

            await _db.Divisions
                .Where(d => d.LeaderId == employeeId)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.LeaderId, (int?)null));

            await _db.Projects
                .Where(p => p.LeaderId == employeeId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.LeaderId, (int?)null));

            await _db.Departments
                .Where(d => d.LeaderId == employeeId)
                .ExecuteUpdateAsync(s => s.SetProperty(d => d.LeaderId, (int?)null));
        }
    }
}
