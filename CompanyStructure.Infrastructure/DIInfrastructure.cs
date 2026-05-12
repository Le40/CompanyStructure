using CompanyStructure.Application.Employees.Interfaces;
using CompanyStructure.Application.Nodes.Interfaces;
using CompanyStructure.Application.Nodes.Interfaces.Validation;
using CompanyStructure.Infrastructure.Data;
using CompanyStructure.Infrastructure.Services.Employees;
using CompanyStructure.Infrastructure.Services.Nodes;
using CompanyStructure.Infrastructure.Services.Nodes.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompanyStructure.Infrastructure
{
    public static class DIInfrastructure
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            // Register infrastructure services here
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IDivisionService, DivisionService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<INodeValidationService, NodeValidationService>();
            return services;
        }
    }
}
