using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Techies.Client.Stats.Api.Model;

namespace techies.client.stats.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ElasticClient _client;
        public const int PageSize = 500;
        public ClientsController(ElasticClient client)
        {
            _client = client;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(int page=1)
        {
            var clients = await _client.SearchAsync<ClientModel>(s => 
            s.From((page - 1) * PageSize).Take(PageSize));            

            return Ok(clients.Documents.ToArray());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(string id)
        {
            return "value";
        }        
    }
}
