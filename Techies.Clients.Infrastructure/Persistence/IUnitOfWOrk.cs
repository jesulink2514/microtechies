using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Techies.Clients.Infrastructure.Persistence
{
    public interface IUnitOfWOrk
    {
        Task<int> SaveAsync();
    }
}
