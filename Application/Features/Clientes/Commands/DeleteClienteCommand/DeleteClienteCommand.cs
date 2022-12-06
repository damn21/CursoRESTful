using Application.Exceptions;
using Application.Interfaces;
using Application.Wrappers;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Commands.DeleteClienteCommand
{
    public class DeleteClienteCommand : IRequest<Response<int>>
    {
        public int Id { get; set; }

        public class DeleteClienteCommandHandler : IRequestHandler<DeleteClienteCommand, Response<int>>
        {
            private readonly IRepositoryAsync<Cliente> repositoryAsync;

            public DeleteClienteCommandHandler(IRepositoryAsync<Cliente> repositoryAsync)
            {
                this.repositoryAsync = repositoryAsync;
            }

            public async Task<Response<int>> Handle(DeleteClienteCommand request, CancellationToken cancellationToken)
            {
                var cliente = await repositoryAsync.GetByIdAsync(request.Id);

                if (cliente == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
                }

                await repositoryAsync.DeleteAsync(cliente);

                return new Response<int>(cliente.Id);
            }
        }
    }
}
