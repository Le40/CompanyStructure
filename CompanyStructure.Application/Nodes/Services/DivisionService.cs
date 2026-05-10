using CompanyStructure.Application.Common.Extentions;
using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Nodes.Services
{
    public class DivisionService : NodeService<Division>, IDivisionService
    {
        public DivisionService(
            AppDbContext db, 
            INodeValidationService validation,
            ILogger<DivisionService> logger) 
            : base(db, validation, logger) 
        { 
        }
        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int companyId)
        {
            _logger.LogInformation("Creating division with name {Name} and code {Code} for company {CompanyId}", dto.Name, dto.Code, companyId);
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);

            if (!companyExists)
            {
                _logger.LogWarning("Company with id {CompanyId} does not exist", companyId);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<Company>());
            }

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, companyId);

            if (!leaderValidation.Success)
                return ServiceResult<NodeResponse>.Fail(leaderValidation.Error!);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<Division>(dto.Code!,companyId);

            if (!codeValidation.Success)
                return ServiceResult<NodeResponse>.Fail(codeValidation.Error!);

            var division = dto.Adapt<Division>();
            division.CompanyId = companyId;

            _db.Divisions.Add(division);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Division with id {DivisionId} created successfully", division.Id);
            return ServiceResult<NodeResponse>.Ok(
                division.Adapt<NodeResponse>());
        }

        public async Task<ServiceResult<List<NodeResponse>>> GetAllAsync(int companyId, PaginationQuery pagination)
        {
            var nodeExists = await _db.Companies.AnyAsync(n => n.Id == companyId);
            if (!nodeExists)
                return ServiceResult<List<NodeResponse>>.Fail(ServiceErrors.NotFound<Company>());

            var divisions = await _db.Divisions
                .Where(d => d.CompanyId == companyId)
                .ToPagedResultAsync<Division, NodeResponse>(pagination.Page, pagination.PageSize);

            return ServiceResult<List<NodeResponse>>.Ok(divisions.Adapt<List<NodeResponse>>());
        }
    }
}
