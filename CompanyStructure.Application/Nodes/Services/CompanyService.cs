using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Employees;
using CompanyStructure.Application.Common.Extentions;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CompanyStructure.Application.Nodes.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _db;
        private readonly INodeValidationService _validation;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(AppDbContext db, INodeValidationService validation, ILogger<CompanyService> logger)
        {
            _db = db;
            _validation = validation;
            _logger = logger;
        }

        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateNodeRequest dto)
        {
            _logger.LogInformation("Creating company with name: {Name} and code: {Code}", dto.Name, dto.Code);
            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code);

            if (codeExists)
            {
                _logger.LogWarning("Failed to create company. Company with code {Code} already exists.", dto.Code);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.DuplicateCode<Company>());
            }

            if (dto.LeaderId != null)
            {
                _logger.LogWarning("Failed to create company. Company director cannot be assigned when creating a new company.");
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.CompanyLeaderCannotBeAssignedOnCreate);
            }

            var company = dto.Adapt<Company>();

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Company created successfully with id: {Id}", company.Id);
            return ServiceResult<NodeResponse>.Ok(
                company.Adapt<NodeResponse>());
        }
            
        public async Task<ServiceResult<List<NodeResponse>>> GetAllAsync(PaginationQuery pagination)    
        {
            var companies = await _db.Companies.ToPagedResultAsync<Company, NodeResponse>(pagination.Page, pagination.PageSize); ;
                
            return ServiceResult<List<NodeResponse>>.Ok(companies.Adapt<List<NodeResponse>>());
        }

        public async Task<ServiceResult<NodeResponse?>> GetByIdAsync(int id)
        {
            var node = await _db.Companies.FindAsync(id);
            if (node == null)
            {
                return ServiceResult<NodeResponse?>.Fail(ServiceErrors.NotFound<Company>());
            }
            return ServiceResult<NodeResponse?>.Ok(node.Adapt<NodeResponse>());
        }


        public async Task<ServiceResult<NodeResponse>> UpdateAsync(int id, UpdateNodeRequest dto)
        {
            var node = await _db.Companies.FindAsync(id);

            if (node == null)
            {
                _logger.LogWarning("Failed to update company. Company with id {Id} was not found.", id);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<Company>());
            }

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, id);

            if (!leaderValidation.Success)
            {
                return ServiceResult<NodeResponse>.Fail(leaderValidation.Error!);
            }
            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code && d.Id != id);

            if (codeExists)
            {
                _logger.LogWarning("Failed to update company. Company with code {Code} already exists.", dto.Code);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.DuplicateCode<Company>());
            }

            dto.Adapt(node);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Company with id {Id} updated successfully.", id);
            return ServiceResult<NodeResponse>.Ok(node.Adapt<NodeResponse>());
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
