using Application.DTOs;
using Application.Interfaces;
using Application.Specifications;
using Application.Wrappers;
using AutoMapper;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Clientes.Queries.GetAllClients
{
    public class GetAllClientQuery : IRequest<Pagination<List<ClienteDto>>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public class GetAllClientQueryHandler : IRequestHandler<GetAllClientQuery, Pagination<List<ClienteDto>>>
        {
            private readonly IRepositoryAsync<Cliente> repositoryAsync;
            private readonly IMapper mapper;

            public GetAllClientQueryHandler(IRepositoryAsync<Cliente> repositoryAsync, IMapper mapper)
            {
                this.repositoryAsync = repositoryAsync;
                this.mapper = mapper;
            }

            public async Task<Pagination<List<ClienteDto>>> Handle(GetAllClientQuery request, CancellationToken cancellationToken)
            {
                var listClient = await repositoryAsync.ListAsync(new PagedClientSpecification(request.PageSize, request.PageNumber, request.Nombre, request.Apellido));
                var dto = mapper.Map<List<ClienteDto>>(listClient);

                return new Pagination<List<ClienteDto>>(dto, request.PageNumber, request.PageSize);
            }
        }
    }
}
