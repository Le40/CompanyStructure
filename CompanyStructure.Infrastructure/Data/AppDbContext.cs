using CompanyStructure.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CompanyStructure.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configure relationships and constraints if needed
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Employees)
                .WithOne(e => e.Company)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Company>()
                .HasOne(c => c.Leader)
                .WithMany()
                .HasForeignKey(c => c.LeaderId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<Company>()
                .HasMany(c => c.Divisions)
                .WithOne(d => d.Company)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Division>()
                .HasOne(d => d.Company)
                .WithMany(c => c.Divisions)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Division>()
                .HasOne(d => d.Leader)
                .WithMany()
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Division)
                .WithMany(d => d.Projects)
                .HasForeignKey(p => p.DivisionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Leader)
                .WithMany()
                .HasForeignKey(p => p.LeaderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Project)
                .WithMany(d => d.Departments)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Leader)
                .WithMany()
                .HasForeignKey(d => d.LeaderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Email)
                .IsUnique();
        }
    }
}
