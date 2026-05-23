using CompanyStructure.Infrastructure.Extensions;
using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Interfaces.Validation;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Infrastructure.Services.Nodes
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

        public async Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(int companyId, PaginationQuery pagination)
        {
            var nodeExists = await _db.Companies.AnyAsync(n => n.Id == companyId);
            if (!nodeExists)
                return ServiceResult<PagedResult<NodeResponse>>.Fail(ServiceErrors.NotFound<Company>(companyId));

            var divisions = await _db.Divisions
                .Where(d => d.CompanyId == companyId)
                .ToPagedResultAsync<Division, NodeResponse>(pagination.Page, pagination.PageSize);

            return ServiceResult<PagedResult<NodeResponse>>.Ok(divisions);
        }

        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto, int companyId)
        {
            _logger.LogInformation("Creating division with name {Name} and code {Code} for company {CompanyId}", dto.Name, dto.Code, companyId);
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);

            if (!companyExists)
            {
                _logger.LogWarning("Company with id {CompanyId} does not exist", companyId);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<Company>(companyId));
            }

            var validation = await _validation.ValidateNodeAsync<Division>(dto.LeaderId, dto.Code, companyId);
            if (!validation.Success)
                return ServiceResult<NodeResponse>.Fail(validation.Error!);

            var division = dto.Adapt<Division>();
            division.CompanyId = companyId;

            _db.Divisions.Add(division);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Division with id {DivisionId} created successfully", division.Id);
            return ServiceResult<NodeResponse>.Ok(
                division.Adapt<NodeResponse>());
        }

        private async Task<ServiceResult<bool>> GetDivisionCreateContextAsync(int companyId)
        {
            var companyExists = await _db.Companies.AnyAsync(c => c.Id == companyId);

            if (!companyExists)
            {
                _logger.LogWarning("Company with id {CompanyId} does not exist", companyId);
                return ServiceResult<bool>.Fail(ServiceErrors.NotFound<Company>(companyId));
            }

            return ServiceResult<bool>.Ok(true);
        }
    }
}
