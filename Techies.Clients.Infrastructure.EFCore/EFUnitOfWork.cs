using System;
using System.Threading.Tasks;
using Techies.Clients.Infrastructure.Persistence;

namespace Techies.Clients.Infrastructure.EFCore
{
    public class EFUnitOfWork: IUnitOfWOrk, IDisposable
    {
        private readonly ClientsDbContext _context;

        public EFUnitOfWork(ClientsDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public Task<int> SaveAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
