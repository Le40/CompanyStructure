using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace CompanyStructure.Application.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _db;

        public CompanyService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto)
        {
            if (dto.LeaderId != null)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company director cannot be assigned when creating a new company. Create employees first, then update company leader.",
                    ServiceErrorType.Validation);
            }

            var codeExists = await _db.Companies
                .AnyAsync(d => d.Code == dto.Code);

            if (codeExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company with this code already exists.",
                    ServiceErrorType.Conflict);
            }

            var company = dto.Adapt<Company>();

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                company.Adapt<GetOrganisationNodeDTO>());
        }
            
        public async Task<List<GetOrganisationNodeDTO>> GetAllAsync()
        {
            var companies = await _db.Companies.ToListAsync();
                
            return companies.Adapt<List<GetOrganisationNodeDTO>>();
        }
    }
}
