using Application.DTOs.Users;
using Application.Interfaces;
using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Authentication.RegisterCommand
{
    public class RegisterCommand : IRequest<Response<string>>
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Origin { get; set; }
    }

    public class RegistercommandHandler : IRequestHandler<RegisterCommand, Response<string>>
    {
        private readonly IAccountServices accountServices;

        public RegistercommandHandler(IAccountServices accountServices)
        {
            this.accountServices = accountServices;
        }

        public async Task<Response<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            return await accountServices.RegisterAsync(new RegisterRequest
            {
                Email = request.Email,
                Password = request.Password,
                ConfirmPassword = request.ConfirmPassword,
                UserName = request.UserName,
                Nombre = request.Nombre,
                Apellido = request.Apellido
            }, request.Origin);
        }
    }
}
