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
using System.Security.Claims;
using System.Security.Cryptography;

namespace DotNetExam.Controllers
{
    [ApiController]
    [Route("accounts")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly CryptographyService _cryptographyService;
        public AccountsController(IMediator mediator, ITokenService token, CryptographyService crypto)
        {
            _mediator = mediator;
            _tokenService = token;
            _cryptographyService = crypto;
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
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
            if (user.PasswordHash != request.Password)
            {
                return BadRequest(new { message = "Wrong password!" } );
            }
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

            return Ok(new AuthResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = accessToken,
                RefreshToken = user.RefreshToken
            });
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<User>>> AllUsers()
        {
            return await _mediator.Send(new GetAllUsersQuery());
        }
        //[HttpPost("register")]
        //public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        //{
            //if(ModelState.IsValid) return BadRequest(ModelState);
            //var user = new IdentityUser<long>()
            //{
            //    Email = request.Email,
            //    PasswordHash = request.PasswordHash,
            //    UserName = request.UserName
            //};
            //var result = await _userManager.CreateAsync(user, request.PasswordHash);
            //foreach (var error in result.Errors)
            //{
            //    ModelState.AddModelError(error.Code, error.Description);
            //}
            //if (!result.Succeeded) return BadRequest(request);
            //var findUser = await _mediator.Send(new GetUserByEmailQuery(request.Email));

            //if (findUser == null) 
            //{
            //    return BadRequest(request);
            //}
            //await _userManager.AddToRoleAsync(findUser, RolesConst.Member);
        //    return Ok();
        //}
    }
}
