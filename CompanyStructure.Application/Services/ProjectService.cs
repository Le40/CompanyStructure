using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CompanyStructure.Application.Services
{
    public class ProjectService : OrganisationNodeService<Project>, IProjectService
    {
        public ProjectService(
            AppDbContext db,
            IOrganisationNodeValidationService validation)
            : base(db, validation)
        {
        }

        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int divisionId)
        {
            var divisionExists = await _db.Divisions.AnyAsync(d => d.Id == divisionId);

            if (!divisionExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Division does not exist.",
                    ServiceErrorType.NotFound);
            }

            var companyId = await _db.Divisions
                .Where(d => d.Id == divisionId)
                .Select(d => d.CompanyId)
                .FirstOrDefaultAsync();

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, companyId);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    leaderValidation.Error!,
                    leaderValidation.ErrorType!.Value);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<Project>(dto.Code!, companyId);

            if (!codeValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    codeValidation.Error!,
                    codeValidation.ErrorType!.Value);

            var project = dto.Adapt<Project>();
            project.DivisionId = divisionId;

            _db.Projects.Add(project);
            await _db.SaveChangesAsync();

            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                project.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int divisionId)
        {
            var projects = await _db.Projects.Where(d => d.DivisionId == divisionId).ToListAsync();
            return ServiceResult<List<GetOrganisationNodeDTO>>.Ok(projects.Adapt<List<GetOrganisationNodeDTO>>());
        }
    }
}
