using CompanyStructure.Infrastructure.Data;
using CompanyStructure.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Application.Services
{
    public class OrganizationNodeService<T> : IOrganisationNodeService<T> 
        where T : class, IOrganizationNode
    {
        private readonly AppDbContext _db;

        public OrganizationNodeService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task<T> CreateAsync(T node)
        {
            _db.Set<T>().Add(node);
            await _db.SaveChangesAsync();
            return node;
        }

        public async Task<T?> UpdateAsync(int id, T updatedNode)
        {
            var existingNode = await _db.Set<T>().FindAsync(id);
            if (existingNode == null)
            {
                return null;
            }
            _db.Entry(existingNode).CurrentValues.SetValues(updatedNode);
            await _db.SaveChangesAsync();
            return existingNode;
        }

        public async Task DeleteAsync(int id)
        {
            var node = await _db.Set<T>().FindAsync(id);
            if (node == null)
                throw new Exception("Not found");

            _db.Set<T>().Remove(node);
            await _db.SaveChangesAsync();
        }
    }
}
