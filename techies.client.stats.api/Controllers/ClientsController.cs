using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Techies.Client.Stats.Api.Model;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> Get(string search=null, int page = 1, int pageSize = 500)
        {
            var datas = await _client.SearchAsync<ClientModel>(s => s.Index("clientsmodel")
            .MatchAll()
            .From((page - 1)*pageSize)
            .Take(pageSize)
            //.Sort(sort => sort.Ascending(f=> f.LastName).Ascending(f=> f.FirstName))            
            );
            
            return Ok(datas.Documents.ToArray());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]        
        public async Task<IActionResult> Get(string id)
        {
            var datas = await _client.SearchAsync<ClientModel>(s => s.Index("clientsmodel")
            .Query(q=> q.Ids(i=> i.Values(id)))
            .Take(1));

            var client = datas.Documents.FirstOrDefault();

            if(client == null) return NotFound();

            return Ok(client);
        }
    }
}
