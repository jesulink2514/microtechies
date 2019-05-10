using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Techies.Client.Stats.Api.Application;

namespace techies.client.stats.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientApplicationService _client;

        public ClientsController(IClientApplicationService clientApplicationService)
        {
            _client = clientApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string search=null, int page = 1, int pageSize = 500)
        {
            var data = await _client.ListClients(search,page,pageSize);
            return Ok(data);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        public async Task<IActionResult> Get(string id)
        {            
            var client = await _client.GetClientById(id);            

            if (client == null) return NotFound();

            return Ok(client);
        }
    }
}
