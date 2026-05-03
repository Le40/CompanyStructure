using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services
{
    public class DivisionService : IDivisionService
    {
        private readonly AppDbContext _db;

        public DivisionService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int companyId)
        {
            var companyExists = await _db.Companies
                .AnyAsync(c => c.Id == companyId);

            if (!companyExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Company does not exist.",
                    ServiceErrorType.NotFound);
            }

            var codeExists = await _db.Divisions
                .AnyAsync(d => d.CompanyId == companyId && d.Code == dto.Code);

            if (codeExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Division with this code already exists in this company.",
                    ServiceErrorType.Conflict);
            }

            if (dto.LeaderId != null)
            {
                var leaderValid = await _db.Employees
                    .AnyAsync(e =>
                        e.Id == dto.LeaderId.Value &&
                        e.CompanyId == companyId);

                if (!leaderValid)
                {
                    return ServiceResult<GetOrganisationNodeDTO>.Fail(
                        "Leader must be an employee of the same company.",
                        ServiceErrorType.Validation);
                }
            }

            var division = dto.Adapt<Division>();
            division.CompanyId = companyId;

            _db.Divisions.Add(division);
            await _db.SaveChangesAsync();

            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                division.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<List<GetOrganisationNodeDTO>> GetAllAsync(int companyId)
        {
            var query = _db.Divisions.AsQueryable();

            query = query.Where(d => d.CompanyId == companyId);

            var entities = await query.ToListAsync();
            return entities.Adapt<List<GetOrganisationNodeDTO>>();
        }
    }
}
