using CompanyStructure.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace CompanyStructure.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        int page,
        int pageSize)
        {
            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectToType<TDto>()
                .ToListAsync();

            return new PagedResult<TDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
