using DotNetExam.Entities;
using DotNetExam.MediatR.Commands;
using DotNetExam.MediatR.Handlers;
using DotNetExam.MediatR.Queries;
using DotNetExam.Models;
using DotNetExam.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DotNetExam.Controllers
{
    [ApiController]
    [Route("accounts")]
    [Produces("application/json")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenService _tokenService;
        private readonly CookiesService _cookiesService;
        public AccountsController(IMediator mediator, ITokenService token, CookiesService cookiesService)
        {
            _mediator = mediator;
            _tokenService = token;
            _cookiesService = cookiesService;
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
        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refToken = _cookiesService.GetRefreshTokenFromCookie(Request);
            if (refToken == null)
            {
                return BadRequest("Refresh token not found!");
            }
            var email = _tokenService.GetEmailFromAccessToken(Request);
            if (email == null) return BadRequest("Bad access token: " + email);

            var user = await _mediator.Send(new GetUserByEmailQuery(email));
            if (user == null) { return BadRequest("User not found!"); }

            if (user.RefreshToken != refToken) { return BadRequest("Refresh token not valid!"); }
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            var response = new RefreshResponse
                (
                    _tokenService.GenerateAccessToken(user),
                    user.RefreshToken
                );
            await _mediator.Send(new EditUserCommand(user));

            return Ok(response);
        }
        private async Task<AuthResponse> Authenticate(User user)
        {
            var accessToken = _tokenService.GenerateAccessToken(user);
            user.RefreshToken = _tokenService.GenerateRefreshToken();
            await _mediator.Send(new EditUserCommand(user));
            _cookiesService.AddRefreshCookieToResponse(Response, user.RefreshToken);
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
