using DotNetExam.Entities;
using DotNetExam.MediatR.Commands;
using DotNetExam.MediatR.Handlers;
using DotNetExam.MediatR.Queries;
using DotNetExam.Models;
using DotNetExam.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.OpenApi.Attributes;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace DotNetExam.Controllers
{
    [ApiController]
    [Route("accounts")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        public AccountsController(IMediator mediator, ITokenService token)
        {
            _mediator = mediator;
            _tokenService = token;
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _mediator.Send(new GetUserByEmailQuery(request.Email));
            if (user == null)
            {
                return BadRequest(new { message = "Wrong login!" });
            }
            if (user.PasswordHash != request.PasswordHash)
            {
                return BadRequest(new { message = "Wrong password!" });
            }
            return Ok(await Authenticate(user));
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<User>>> AllUsers()
        {
            return await _mediator.Send(new GetAllUsersQuery());
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = new User()
            {
                Email = request.Email,
                PasswordHash = request.PasswordHash,
                UserName = request.UserName,
                RefreshToken = "token",
                Role = request.Role
            };
            await _mediator.Send(new AddUserCommand(user));

            var findUser = await _mediator.Send(new GetUserByEmailQuery(request.Email));
            if (findUser == null)
            {
                return BadRequest(request);
            }
            return Ok(await Authenticate(findUser));
        }
        private async Task<AuthResponse> Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, Convert.ToInt32(user.Role).ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(claims, "Bearer");
            var accessToken = _tokenService.GenerateAccessToken(identity);
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            await _mediator.Send(new EditUserCommand(user));
            return new AuthResponse()
            {
                Email = user.Email,
                RefreshToken = user.RefreshToken,
                AccessToken = accessToken,
                Username = user.UserName
            };
        }
    }
}
