using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Employees.DTOs;  

namespace CompanyStructure.Application.Employees.Interfaces
{
    public interface IEmployeeService
    {
        Task<ServiceResult<PagedResult<EmployeeResponse>>> GetAllEmployeesAsync(int companyId, PaginationQuery pagination);
        Task<ServiceResult<EmployeeResponse?>> GetEmployeeByIdAsync(int id);
        Task<ServiceResult<EmployeeResponse>> CreateEmployeeAsync(int companyId, CreateEmployeeRequest dto);
        Task<ServiceResult<EmployeeResponse>> UpdateEmployeeAsync(int id, UpdateEmployeeRequest dto);
        Task<ServiceResult<bool>> DeleteEmployeeAsync(int id);
    }
}
