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
    public class DivisionService : OrganisationNodeService<Division>, IDivisionService
    {
        public DivisionService(
            AppDbContext db, 
            IOrganisationNodeValidationService validation,
            ILogger<DivisionService> logger) 
            : base(db, validation, logger) 
        { 
        }
        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int companyId)
        {
            _logger.LogInformation("Creating division with name {Name} and code {Code} for company {CompanyId}", dto.Name, dto.Code, companyId);
            var companyExists = await _db.Companies
                .AnyAsync(c => c.Id == companyId);

            if (!companyExists)
            {
                _logger.LogWarning("Company with id {CompanyId} does not exist", companyId);
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company does not exist.",
                    ServiceErrorType.NotFound);
            }

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, companyId);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    leaderValidation.Error!,
                    leaderValidation.ErrorType!.Value);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<Division>(dto.Code!,companyId);

            if (!codeValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    codeValidation.Error!,
                    codeValidation.ErrorType!.Value);

            var division = dto.Adapt<Division>();
            division.CompanyId = companyId;

            _db.Divisions.Add(division);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Division with id {DivisionId} created successfully", division.Id);
            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                division.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int companyId)
        {
            var query = _db.Divisions.AsQueryable();

            query = query.Where(d => d.CompanyId == companyId);

            var entities = await query.ToListAsync();
            return ServiceResult<List<GetOrganisationNodeDTO>>.Ok(entities.Adapt<List<GetOrganisationNodeDTO>>());
        }
    }
}
