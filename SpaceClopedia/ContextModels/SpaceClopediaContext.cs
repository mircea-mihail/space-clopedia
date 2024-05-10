using Microsoft.EntityFrameworkCore;
using SpaceClopedia.Models;
using System.Data.Common;

namespace SpaceClopedia.ContextModels
{
    public class SpaceClopediaContext : DbContext
    {
        public SpaceClopediaContext(DbContextOptions<SpaceClopediaContext> options) : base(options) 
        { 

        }
        public DbSet<ArticolModel> Articol { get; set;}
        public DbSet<DomeniuModel> Domeniu { get; set; }
        public DbSet<UtilizatorModel> Utilizator { get; set; }

    }
}
