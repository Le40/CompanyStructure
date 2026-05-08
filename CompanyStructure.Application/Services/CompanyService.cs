using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Results;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _db;
        private readonly IOrganisationNodeValidationService _validation;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(AppDbContext db, IOrganisationNodeValidationService validation, ILogger<CompanyService> logger)
        {
            _db = db;
            _validation = validation;
            _logger = logger;
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto)
        {
            _logger.LogInformation("Creating company with name: {Name} and code: {Code}", dto.Name, dto.Code);
            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code);

            if (codeExists)
            {
                _logger.LogWarning("Failed to create company. Company with code {Code} already exists.", dto.Code);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(ServiceErrors.DuplicateCode<Company>());
            }

            if (dto.LeaderId != null)
            {
                _logger.LogWarning("Failed to create company. Company director cannot be assigned when creating a new company.");
                return ServiceResult<GetOrganisationNodeDTO>.Fail(ServiceErrors.CompanyLeaderCannotBeAssignedOnCreate);
            }

            var company = dto.Adapt<Company>();

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Company created successfully with id: {Id}", company.Id);
            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                company.Adapt<GetOrganisationNodeDTO>());
        }
            
        public async Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync()
        {
            var companies = await _db.Companies.ToListAsync();
                
            return ServiceResult<List<GetOrganisationNodeDTO>>.Ok(companies.Adapt<List<GetOrganisationNodeDTO>>());
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO?>> GetByIdAsync(int id)
        {
            var node = await _db.Companies.FindAsync(id);
            if (node == null)
            {
                return ServiceResult<GetOrganisationNodeDTO?>.Fail(ServiceErrors.NotFound<Company>());
            }
            return ServiceResult<GetOrganisationNodeDTO?>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }


        public async Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto)
        {
            var node = await _db.Companies.FindAsync(id);

            if (node == null)
            {
                _logger.LogWarning("Failed to update company. Company with id {Id} was not found.", id);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(ServiceErrors.NotFound<Company>());
            }

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, id);

            if (!leaderValidation.Success)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(leaderValidation.Error!);
            }
            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code && d.Id != id);

            if (codeExists)
            {
                _logger.LogWarning("Failed to update company. Company with code {Code} already exists.", dto.Code);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(ServiceErrors.DuplicateCode<Company>());
            }

            dto.Adapt(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Company with id {Id} updated successfully.", id);
            return ServiceResult<GetOrganisationNodeDTO>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var node = await _db.Companies.FindAsync(id);
            if (node == null) {
                _logger.LogWarning("Failed to delete company. Company with id {Id} was not found.", id);
                return ServiceResult<bool>.Fail(ServiceErrors.NotFound<Company>());
            }
            _db.Companies.Remove(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Company with id {Id} deleted successfully.", id);
            return ServiceResult<bool>.Ok(true);
        }
    }
}
