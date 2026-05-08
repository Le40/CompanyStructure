using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Application.Results;

namespace CompanyStructure.Application.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<ServiceResult<List<GetEmployeeDTO>>> GetAllEmployeesAsync(int companyId);
        Task<ServiceResult<GetEmployeeDTO?>> GetEmployeeByIdAsync(int id);
        Task<ServiceResult<GetEmployeeDTO>> CreateEmployeeAsync(int companyId, CreateEmployeeDTO dto);
        Task<ServiceResult<GetEmployeeDTO>> UpdateEmployeeAsync(int id, UpdateEmployeeDTO dto);
        Task<ServiceResult<bool>> DeleteEmployeeAsync(int id);
    }
}
