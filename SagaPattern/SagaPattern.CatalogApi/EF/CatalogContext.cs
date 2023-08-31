using Microsoft.EntityFrameworkCore;
using SagaPattern.Choreography.CatalogApi.Models;

namespace SagaPattern.Choreography.CatalogApi.EF
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
        {

        }

        public DbSet<CatalogItem> CatalogItems { get; set; }
    }
}
