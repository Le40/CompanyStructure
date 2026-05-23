using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Employees.DTOs;
using CompanyStructure.Application.Employees.Interfaces;
using CompanyStructure.WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyStructure.WebAPI.Controllers.Employees
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(IEmployeeService _service) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpGet("/api/Companies/{companyId}/[controller]")]
        public async Task<IActionResult> GetAllEmployees([FromRoute] int companyId, [FromQuery] PaginationQuery pagination)
        {
            var result = await _service.GetAllEmployeesAsync(companyId, pagination);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "User")]
        [HttpPost("/api/Companies/{companyId}/[controller]")]
        public async Task<IActionResult> CreateEmployee(int companyId, CreateEmployeeRequest dto)
        {
            var result = await _service.CreateEmployeeAsync(companyId, dto);
            if (!result.Success)
                return result.ToActionResult(this);

            return CreatedAtAction(nameof(GetEmployeeById), new { id = result.Data!.Id }, result.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var result = await _service.GetEmployeeByIdAsync(id);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, UpdateEmployeeRequest dto)
        {
            var result = await _service.UpdateEmployeeAsync(id, dto);
            return result.ToActionResult(this);
        }

        [Authorize(Roles = "Admin")]
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
