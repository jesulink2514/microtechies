using System.Collections.Generic;
using System.Threading.Tasks;
using Techies.Clients.Domain;

namespace Techies.Clients.Infrastructure
{
    public interface IClientsRepository
    {
        void Add(Client newClient);
        Task<List<Client>> ListAll();
    }
}
