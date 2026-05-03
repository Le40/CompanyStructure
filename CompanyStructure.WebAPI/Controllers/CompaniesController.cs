using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CompanyStructure.Application.Services.Interfaces;

namespace CompanyStructure.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class CompaniesController : OrganisationNodeController<Company>
    {
        private readonly ICompanyService _companyService;
        public CompaniesController(
            IOrganisationNodeService<Company> service,
            ICompanyService companyService) : base(service)
        {
            _companyService = companyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllAsync();
            return Ok(companies);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrganisationNodeDTO dto)
        {
            var result = await _companyService.CreateAsync(dto);
            if (!result.Success)
            {
                return BadRequest(new { error = result.Error });
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
        }
    }
}
