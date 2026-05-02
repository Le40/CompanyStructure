using Microsoft.Extensions.DependencyInjection;
using CompanyStructure.Services;

namespace CompanyStructure.Application
{
    public static class DIApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            services.AddScoped<IEmployeeService, EmployeeService>();
            return services;
        }
    }
}
