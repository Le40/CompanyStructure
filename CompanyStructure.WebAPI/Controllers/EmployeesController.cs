using CompanyStructure.Application.DTOs.Employees;
using CompanyStructure.Application.Services;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeeService _service) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] int? companyID)
        {
            return Ok( await _service.GetAllEmployeesAsync(companyID));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var employee = await _service.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(CreateEmployeeDTO dto)
        {
            var result = await _service.CreateEmployeeAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeDTO dto)
        {
            var result = await _service.UpdateEmployeeAsync(id, dto);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }
            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var deletedEmployee = await _service.DeleteEmployeeAsync(id);
            if (deletedEmployee == null)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
