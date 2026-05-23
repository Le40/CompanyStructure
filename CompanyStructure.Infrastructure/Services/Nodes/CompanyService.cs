using CompanyStructure.Application.Common.Pagination;
using CompanyStructure.Application.Common.ServiceResult;
using CompanyStructure.Application.Employees;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Interfaces.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using CompanyStructure.Infrastructure.Extensions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace CompanyStructure.Infrastructure.Services.Nodes
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
            
        public async Task<ServiceResult<PagedResult<NodeResponse>>> GetAllAsync(PaginationQuery pagination)    
        {
            var companies = await _db.Companies.ToPagedResultAsync<Company, NodeResponse>(pagination.Page, pagination.PageSize); ;
                
            return ServiceResult<PagedResult<NodeResponse>>.Ok(companies);
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

        public async Task<ServiceResult<CompanyStructureResponse>> GetStructureByIdAsync(int id)
        {
            var company = await _db.Companies
                .Include(c => c.Divisions)
                    .ThenInclude(d => d.Projects)
                        .ThenInclude(p => p.Departments)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return ServiceResult<CompanyStructureResponse>.Fail(ServiceErrors.NotFound<Company>());
            }
            var structure = company.Adapt<CompanyStructureResponse>();
            return ServiceResult<CompanyStructureResponse>.Ok(structure);
        }

        public async Task<ServiceResult<NodeResponse>> CreateAsync(CreateCompanyRequest dto)
        {
            _logger.LogInformation("Creating company with name: {Name} and code: {Code}", dto.Name, dto.Code);

            var validation = await ValidateCreateAsync(dto);
            if (!validation.Success)
            {
                return ServiceResult<NodeResponse>.Fail(validation.Error!);
            }   

            var company = dto.Adapt<Company>();

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Company created successfully with id: {Id}", company.Id);
            return ServiceResult<NodeResponse>.Ok(
                company.Adapt<NodeResponse>());
        }

        public async Task<ServiceResult<NodeResponse>> UpdateAsync(int id, UpdateNodeRequest dto)
        {
            var node = await _db.Companies.FindAsync(id);

            if (node == null)
            {
                _logger.LogWarning("Failed to update company. Company with id {Id} was not found.", id);
                return ServiceResult<NodeResponse>.Fail(ServiceErrors.NotFound<Company>());
            }

            var validation = await ValidateUpdateAsync(dto, id);
            if (!validation.Success)
            {
                return ServiceResult<NodeResponse>.Fail(validation.Error!);
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

        private async Task<ServiceResult<bool>> ValidateCreateAsync(CreateCompanyRequest dto)
        {
            var codeExists = await _db.Companies.AnyAsync(d => d.Code == dto.Code);
            if (codeExists)
            {
                _logger.LogWarning("Failed to create company. Company with code {Code} already exists.", dto.Code);
                return ServiceResult<bool>.Fail(ServiceErrors.DuplicateCode<Company>());
            }
            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> ValidateUpdateAsync(UpdateNodeRequest dto, int id)
        {
            var leaderValidation = await _validation.ValidateLeaderAsync<Company>(dto.LeaderId, id);

            if (!leaderValidation.Success)
                return leaderValidation;

            var codeExists = await _db.Companies.AnyAsync(d => d.Code == dto.Code && d.Id != id);
            if (codeExists)
            {
                _logger.LogWarning("Failed to create company. Company with code {Code} already exists.", dto.Code);
                return ServiceResult<bool>.Fail(ServiceErrors.DuplicateCode<Company>());
            }
            return ServiceResult<bool>.Ok(true);
        }
    }   
}
