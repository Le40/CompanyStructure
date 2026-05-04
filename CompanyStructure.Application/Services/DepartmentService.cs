using CompanyStructure.Application.DTOs.OrganisationNodes;
using CompanyStructure.Application.Services.Interfaces;
using CompanyStructure.Application.Services.Validation;
using CompanyStructure.Domain.Models;
using CompanyStructure.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CompanyStructure.Application.Services
{
    public class DepartmentService : OrganisationNodeService<Department>, IDepartmentService
    {
        public DepartmentService(   
            AppDbContext db,
            IOrganisationNodeValidationService validation)
            : base(db, validation)
        {
        }
        public async Task<ServiceResult<GetOrganisationNodeDTO>> CreateAsync(CreateOrganisationNodeDTO dto, int projectId)
        {
            var projectExists = await _db.Projects.AnyAsync(p => p.Id == projectId);

            if (!projectExists)
            {
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    "Project does not exist.",
                    ServiceErrorType.NotFound);
            }

            var companyId = await _db.Projects
                .Where(p => p.Id == projectId)
                .Select(p => p.CompanyId)
                .FirstOrDefaultAsync();

            var leaderValidation = await _validation.ValidateLeaderAsync(dto.LeaderId, companyId);

            if (!leaderValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    leaderValidation.Error!,
                    leaderValidation.ErrorType!.Value);

            var codeValidation = await _validation.ValidateCodeIsUniqueAsync<Department>(dto.Code!, companyId);

            if (!codeValidation.Success)
                return ServiceResult<GetOrganisationNodeDTO>.Fail(
                    codeValidation.Error!,
                    codeValidation.ErrorType!.Value);

            var department = dto.Adapt<Department>();
            department.ProjectId = projectId;

            _db.Departments.Add(department);
            await _db.SaveChangesAsync();

            return ServiceResult<GetOrganisationNodeDTO>.Ok(
                department.Adapt<GetOrganisationNodeDTO>());
        }

        public async Task<ServiceResult<List<GetOrganisationNodeDTO>>> GetAllAsync(int projectId)
        {
            var departments = await _db.Departments.Where(d => d.ProjectId == projectId).ToListAsync();
            return ServiceResult<List<GetOrganisationNodeDTO>>.Ok(departments.Adapt<List<GetOrganisationNodeDTO>>());
        }
    }
}
