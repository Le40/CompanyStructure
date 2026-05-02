using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeeService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllEmployees(int? companyID)
        {
            return Ok( await service.GetAllEmployeesAsync(companyID));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await service.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] Employee employee)
        {
            var createdEmployee = await service.CreateEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = createdEmployee.Id }, createdEmployee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            var updatedEmployee = await service.UpdateEmployeeAsync(id, employee);
            if (updatedEmployee == null)
            {
                return NotFound();
            }
            return Ok(updatedEmployee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deletedEmployee = await service.DeleteEmployeeAsync(id);
            if (deletedEmployee == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
