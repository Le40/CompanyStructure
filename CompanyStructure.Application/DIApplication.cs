using Microsoft.Extensions.DependencyInjection;
using CompanyStructure.Application.Services;
using CompanyStructure.Application.Services.Interfaces;

namespace CompanyStructure.Application
{
    public static class DIApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped(typeof(IOrganisationNodeService<>), typeof(OrganizationNodeService<>));
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IDivisionService, DivisionService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            return services;
        }
    }
}
