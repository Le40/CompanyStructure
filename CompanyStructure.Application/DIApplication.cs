using Microsoft.Extensions.DependencyInjection;
using CompanyStructure.Application.Employees.InterFaces;
using CompanyStructure.Application.Employees.Services;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Services;
using CompanyStructure.Application.Nodes.Validation;

namespace CompanyStructure.Application
{
    public static class DIApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            services.AddScoped<IEmployeeService, EmployeeService>();
            //services.AddScoped(typeof(IOrganisationNodeService<>), typeof(OrganisationNodeService<>));
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IDivisionService, DivisionService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<INodeValidationService, NodeValidationService>();
            return services;
        }
    }
}
