using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace CompanyStructure.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _db;

        public EmployeeService(AppDbContext db)
        {
            _db = db;
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
                return ServiceResult<GetEmployeeDTO?>.Fail("Employee not found.", ServiceErrorType.NotFound);

            return ServiceResult<GetEmployeeDTO?>.Ok(employee.Adapt<GetEmployeeDTO>());
        }

        public async Task<ServiceResult<GetEmployeeDTO>> CreateEmployeeAsync(int companyId, CreateEmployeeDTO dto)
        {
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);
            if (!companyExists)
            {
                return ServiceResult<GetEmployeeDTO>.Fail("Company does not exist.", ServiceErrorType.NotFound);
            }

            var emailExists = await _db.Employees.AnyAsync(e => e.Email == dto.Email);
            if (emailExists)
            {
                return ServiceResult<GetEmployeeDTO>.Fail("Email already exists.", ServiceErrorType.Conflict);
            }

            var employee = dto.Adapt<Employee>();
            employee.CompanyId = companyId;

            _db.Employees.Add(employee);
            await _db.SaveChangesAsync();

            return ServiceResult<GetEmployeeDTO>.Ok(employee.Adapt<GetEmployeeDTO>());
        }

        public async Task<ServiceResult<GetEmployeeDTO>> UpdateEmployeeAsync(int id, UpdateEmployeeDTO dto)
        {
            var employee = await _db.Employees.FindAsync(id);
            if (employee == null)
            {
                return ServiceResult<GetEmployeeDTO>.Fail("Employee not found.", ServiceErrorType.NotFound);
            }

            var emailExists = await _db.Employees.AnyAsync(e => e.Email == dto.Email && e.Id != id);
            if (emailExists)
            {
                return ServiceResult<GetEmployeeDTO>.Fail("Email already exists.", ServiceErrorType.Conflict);
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
                return ServiceResult<bool>.Fail("Employee not found.", ServiceErrorType.NotFound);
            }
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);

        }
    }
}
