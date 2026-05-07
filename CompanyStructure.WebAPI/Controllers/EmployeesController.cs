using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.WebAPI.Controllers.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
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
        public async Task<IActionResult> CreateEmployee(int companyId, CreateEmployeeDTO dto)
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
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDTO dto)
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
