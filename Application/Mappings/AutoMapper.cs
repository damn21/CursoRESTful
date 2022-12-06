using Application.DTOs;
using Application.Features.Clientes.Commands.CreateClienteCommand;
using Application.Features.Clientes.Queries.GetAllClientsById;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            #region DTOs

            CreateMap<Cliente, ClienteDto>();

            #endregion

            #region Commands

            CreateMap<CreateClienteCommand, Cliente>();

            #endregion
        }
    }
}
