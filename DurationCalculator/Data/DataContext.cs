using System;
using Microsoft.EntityFrameworkCore;
using DurationCalculator.Models;

namespace DurationCalculator.Data
{
    public partial class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<jobtiming> jobtimings { get; set; }
        public DbSet<stationManagement> stationManagements { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<jobtiming>(entity => {
                entity.ToTable("jobtiming");
                entity.HasKey(x => x.id);
            });

            modelBuilder.Entity<stationManagement>(entity => {
                entity.ToTable("stationManagement");
                entity.HasNoKey();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
