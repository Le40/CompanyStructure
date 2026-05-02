using CompanyStructure.Models;

namespace CompanyStructure.Services
{
    public class EmployeeService : IEmployeeService
    {
        static List<Employee> employees = new List<Employee>
        {
            new Employee { Id = 1, Degree = "Ing.", Name = "John",  Surname = "Doe", Email = "JohnDoe@Company.com", PhoneNumber = "0910121212",  CompanyId = 1},
            new Employee { Id = 2, Degree = "Mgr.", Name = "Jane",  Surname = "Smith", Email = "JaneSmith@Company.com:", PhoneNumber = "0910121213",  CompanyId = 1},
            new Employee { Id = 3, Degree = "PhD.", Name = "Alice", Surname = "Johnson", Email = "AliceJohnson@Comapany.com", PhoneNumber = "0910121214",  CompanyId = 2},

        };
        public async Task<List<Employee>> GetAllEmployeesAsync(int? companyID)
            => await Task.FromResult(employees.ToList());

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await Task.FromResult(employees.FirstOrDefault(e => e.Id == id));
        }

        public Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<Employee> UpdateEmployeeAsync(int id, Employee employee)
        {
            throw new NotImplementedException();
        }

        public Task<Employee> DeleteEmployeeAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
