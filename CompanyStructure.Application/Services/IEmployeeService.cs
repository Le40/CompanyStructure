using CompanyStructure.Models;

namespace CompanyStructure.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllEmployeesAsync(int? companyID);
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<Employee> CreateEmployeeAsync(Employee employee);
        Task<Employee> UpdateEmployeeAsync(int id, Employee employee);
        Task<Employee> DeleteEmployeeAsync(int id);
    }
}
