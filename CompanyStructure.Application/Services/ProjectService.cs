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
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _db;

        public ProjectService(AppDbContext db)
        {
            _db = db;
        }

        public Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int divisionId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetOrganisationNodeDTO>> GetAllAsync(int divisionId)
        {
            var projects = await _db.Projects.Where(d => d.DivisionId == divisionId).ToListAsync();
            return projects.Adapt<List<GetOrganisationNodeDTO>>();
        }
    }
}
