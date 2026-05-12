using Microsoft.Extensions.DependencyInjection;

namespace CompanyStructure.Application
{
    public static class DIApplication
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Register application services here
            return services;
        }
    }
}
