using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Techies.Clients.Domain;
using Techies.Clients.Infrastructure.Persistence;

namespace Techies.Clients.Infrastructure.EFCore
{
    public class ClientsDbContext: DbContext
    {
        public ClientsDbContext(DbContextOptions<ClientsDbContext> options):base(options)
        {
        }
        public DbSet<Client> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasKey(e=>e.Id);

            base.OnModelCreating(modelBuilder);
        }
    }
}
