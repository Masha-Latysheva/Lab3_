using System.Collections.Generic;
using Lab3_.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab3_.Data
{
    public class LogisticContext : DbContext
    {
        public LogisticContext(DbContextOptions<LogisticContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Transportation> Transportations { get; set; }

        public DbSet<Route> Routes { get; set; }

        public DbSet<Rate> Rates { get; set; }

        public DbSet<Point> Points { get; set; }

        public DbSet<Organization> Organizations { get; set; }

        public DbSet<Driver> Drivers { get; set; }

        public DbSet<Cargo> Cargos { get; set; }

        public DbSet<Car> Cars { get; set; }

        public DbSet<RoutePoint> RoutePoints { get; set; }

        public void DetachEntities<TEntity>(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities) DetachEntity(entity);
        }

        public void DetachEntity<TEntity>(TEntity entity)
        {
            Entry(entity).State = EntityState.Detached;
        }
    }
}