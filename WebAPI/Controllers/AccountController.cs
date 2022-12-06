using Application.DTOs.Users;
using Application.Features.Authentication;
using Application.Features.Authentication.AuthenticateCommand;
using Application.Features.Authentication.RegisterCommand;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseApiController
    {
        [HttpPost("authenticate")]
        public async Task<IActionResult> AuthenticateAsync(AuthenticationRequest request)
        {
            return Ok(await Mediator.Send(new AuthenticateCommand
            {
                Email = request.Email,
                Password = request.Password,
                IpAddress = GenerateIpAddress()
            }));
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            return Ok( await Mediator.Send(new RegisterCommand
            {
                Email = request.Email,
                Password = request.Password,
                Apellido = request.Apellido,
                Nombre = request.Nombre,
                UserName = request.UserName,
                ConfirmPassword = request.ConfirmPassword,
                Origin = Request.Headers["origin"]
            }));
        }

        private string GenerateIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
