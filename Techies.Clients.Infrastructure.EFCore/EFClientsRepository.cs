using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techies.Clients.Domain;

namespace Techies.Clients.Infrastructure.EFCore
{
    public class EFClientsRepository : IClientsRepository, IDisposable
    {
        private readonly ClientsDbContext _dbContext;

        public EFClientsRepository(ClientsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(Client newClient)
        {
            _dbContext.Clients.Add(newClient);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

        public async Task<List<Client>> ListAll()
        {
            return await _dbContext.Clients.ToListAsync();
        }
    }
}
