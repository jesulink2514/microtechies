using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Techies.Clients.ApplicationServices.Abstract;
using Techies.Clients.DTOs.Request;
using Microsoft.AspNetCore.Http;
using Techies.Clients.DTOs.Responses;
using System;

namespace techies.client.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientApplicationService _clientApplicationService;

        public ClientsController(IClientApplicationService clientApplicationService)
        {
            _clientApplicationService = clientApplicationService;
        }        
        
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status400BadRequest)]        
        public async Task<IActionResult> Post([FromBody] RegisterClient client)
        {
            client.User = User.Identity.Name;
            var result = await _clientApplicationService.Register(client);
            if(result.IsCorrect) return CreatedAtAction("Get", new { id = result.Data});
            return BadRequest(result);
        }        


        [HttpGet]
        [ProducesResponseType(typeof(Techies.Clients.Domain.Client),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(string id)
        {            
            if(id == null) return BadRequest();
            if(!Guid.TryParse(id,out var clientId)) return BadRequest();

            var result = await _clientApplicationService.GetById(clientId);
            if (result == null) return NotFound();            
            return Ok(result);
        }
    }
}
