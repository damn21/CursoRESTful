using Application.DTOs;
using Application.Interfaces;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Queries.GetAllClientsById
{
    public class GetClientByIdQuery : IRequest<Response<ClienteDto>>
    {
        public int Id { get; set; }

        public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, Response<ClienteDto>>
        {
            private readonly IRepositoryAsync<Cliente> repositoryAsync;
            private readonly IMapper mapper;
            public GetClientByIdQueryHandler(IRepositoryAsync<Cliente> repositoryAsync, IMapper mapper)
            {
                this.repositoryAsync = repositoryAsync;
                this.mapper = mapper;
            }


            public async Task<Response<ClienteDto>> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
            {
                var cliente = await repositoryAsync.GetByIdAsync(request.Id);

                if (cliente == null)
                {
                    throw new KeyNotFoundException($"Registro no encontrado con el id {request.Id}");
                }


                var dto = mapper.Map<ClienteDto>(cliente);

                return new Response<ClienteDto>(dto);
            }
        }
    }
}
