using CompanyStructure.Application.Employees;
using CompanyStructure.Application.Employees.InterFaces;
using CompanyStructure.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeeService _service) : ControllerBase
    {

        [HttpGet("/api/Companies/{companyId}/[controller]")]
        public async Task<IActionResult> GetAllEmployees([FromRoute] int companyId)
        {
            var result = await _service.GetAllEmployeesAsync(companyId);
            return result.ToActionResult(this);
        }

        [HttpPost("/api/Companies/{companyId}/[controller]")]
        public async Task<IActionResult> CreateEmployee(int companyId, CreateEmployeeRequest dto)
        {
            var result = await _service.CreateEmployeeAsync(companyId, dto);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _service.GetEmployeeByIdAsync(id);
            return result.ToActionResult(this);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeRequest dto)
        {
            var result = await _service.UpdateEmployeeAsync(id, dto);
            return result.ToActionResult(this);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _service.DeleteEmployeeAsync(id);
            if (!result.Success)
                return result.ToActionResult(this);

            return NoContent();
        }
    }
}
