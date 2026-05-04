using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace CompanyStructure.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _db;
            private readonly IOrganisationNodeValidationService _validation;

        public CompanyService(AppDbContext db, IOrganisationNodeValidationService validation)
        {
            _db = db;
            _validation = validation;
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto)
        {
            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code);

            if (codeExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company with this code already exists.",
                    ServiceErrorType.Conflict);
            }

            if (dto.LeaderId != null)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company director cannot be assigned when creating a new company. Create employees first, then update company leader.",
                    ServiceErrorType.Validation);
            }

            var company = dto.Adapt<Company>();

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

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
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Node not found.",
                    ServiceErrorType.NotFound);

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, id);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    leaderValidation.Error!,
                    leaderValidation.ErrorType!.Value);

            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code && d.Id != id);

            if (codeExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company with this code already exists.",
                    ServiceErrorType.Conflict);
            }

            var updatedNode = dto.Adapt(node);
            _db.Entry(node).CurrentValues.SetValues(updatedNode);
            await _db.SaveChangesAsync();
            return ServiceResult<GetOrganisationNodeDTO>.Ok(node.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            var node = await _db.Companies.FindAsync(id);
            if (node == null)
                return ServiceResult<bool>.Fail("Node was not found", ServiceErrorType.NotFound);

            _db.Companies.Remove(node);
            await _db.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
    }
}
