using Application.Enums;
using Application.Features.Clientes.Commands.CreateClienteCommand;
using Application.Features.Clientes.Commands.DeleteClienteCommand;
using Application.Features.Clientes.Commands.UpdateClienteCommand;
using Application.Features.Clientes.Queries.GetAllClients;
using Application.Features.Clientes.Queries.GetAllClientsById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.v1
{
    [ApiVersion("1.0")]
    public class ClienteController : BaseApiController
    {
        //GET: api/<Controller>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllClientParameters filter)
        {
            return Ok(await Mediator.Send(new GetAllClientQuery 
            { 
                PageNumber = filter.PageNumber, 
                PageSize = filter.PageSize,
                Nombre = filter.Nombre,
                Apellido = filter.Apellido
            }));
        }
        //GET: api/<Controller>/2
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {

            return Ok(await Mediator.Send(new GetClientByIdQuery { Id = id}));
        }

        //POST api/<Controller>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post(CreateClienteCommand createcommand)
        {
            return Ok(await Mediator.Send(createcommand));
        }

        //PUT api/<Controller>/2
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, UpdateClienteCommand updatecommand)
        {
            if (id != updatecommand.Id)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(updatecommand));
        }

        //DELETE api/<Controller>/2
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteClienteCommand { Id = id}));
        }
    }
}
