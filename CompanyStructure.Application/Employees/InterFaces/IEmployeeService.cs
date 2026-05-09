using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;

namespace CompanyStructure.Application.Employees.InterFaces
{
    public interface IEmployeeService
    {
        Task<ServiceResult<List<EmployeeResponse>>> GetAllEmployeesAsync(int companyId, PaginationQuery pagination);
        Task<ServiceResult<EmployeeResponse?>> GetEmployeeByIdAsync(int id);
        Task<ServiceResult<EmployeeResponse>> CreateEmployeeAsync(int companyId, CreateEmployeeRequest dto);
        Task<ServiceResult<EmployeeResponse>> UpdateEmployeeAsync(int id, UpdateEmployeeRequest dto);
        Task<ServiceResult<bool>> DeleteEmployeeAsync(int id);
    }
}
