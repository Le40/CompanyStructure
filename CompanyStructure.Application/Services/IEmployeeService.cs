using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Domain.Models;

namespace CompanyStructure.Application.Services
{
    public interface IEmployeeService
    {
        Task<List<GetEmployeeDTO>> GetAllEmployeesAsync(int? companyID);
        Task<GetEmployeeDTO?> GetEmployeeByIdAsync(int id);
        Task<ServiceResult<GetEmployeeDTO>> CreateEmployeeAsync(CreateEmployeeDTO dto);
        Task<ServiceResult<GetEmployeeDTO>> UpdateEmployeeAsync(int id, UpdateEmployeeDTO dto);
        Task<ServiceResult<bool>> DeleteEmployeeAsync(int id);
    }
}
