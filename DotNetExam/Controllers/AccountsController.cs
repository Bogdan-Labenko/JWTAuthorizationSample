using DotNetExam.Entities;
using DotNetExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DotNetExam.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class AccountsController
    {

        [HttpPost("register")]
        public ActionResult<AuthResponse> Register([FromBody] RegisterRequest request)
        {
            
        }
    }
}
