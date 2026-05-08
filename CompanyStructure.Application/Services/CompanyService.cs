using CompanyStructure.Application.DTOs.OrganisationNodes;
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
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company with this code already exists.",
                    ServiceErrorType.Conflict);
            }

            if (dto.LeaderId != null)
            {
                _logger.LogWarning("Failed to create company. Company director cannot be assigned when creating a new company.");
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company director cannot be assigned when creating a new company. Create employees first, then update company leader.",
                    ServiceErrorType.Validation);
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
                return ServiceResult<GetOrganisationNodeDTO?>.Fail("Node not found", ServiceErrorType.NotFound);
            }
            return ServiceResult<GetOrganisationNodeDTO?>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }


        public async Task<ServiceResult<GetOrganisationNodeDTO>> UpdateAsync(int id, UpdateOrganisationNodeDTO dto)
        {
            var node = await _db.Companies.FindAsync(id);

            if (node == null)
            {
                _logger.LogWarning("Failed to update company. Company with id {Id} was not found.", id);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Node not found.",
                    ServiceErrorType.NotFound);
            }

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, id);

            if (!leaderValidation.Success)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    leaderValidation.Error!,
                    leaderValidation.ErrorType!.Value);
            }
            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code && d.Id != id);

            if (codeExists)
            {
                _logger.LogWarning("Failed to update company. Company with code {Code} already exists.", dto.Code);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company with this code already exists.",
                    ServiceErrorType.Conflict);
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
                return ServiceResult<bool>.Fail("Node was not found", ServiceErrorType.NotFound);
            }
            _db.Companies.Remove(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Company with id {Id} deleted successfully.", id);
            return ServiceResult<bool>.Ok(true);
        }
    }
}
