using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Techies.Clients.Infrastructure.Persistence;

namespace Techies.Clients.Infrastructure.EFCore
{
    public class EFUnitOfWork: IUnitOfWOrk, IDisposable
    {
        private readonly ClientsDbContext _context;
        private readonly ILogger<EFUnitOfWork> _logger;

        public EFUnitOfWork(ClientsDbContext context,ILogger<EFUnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        public async Task<int> SaveAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return 0;
            }
            
        }
    }
}
