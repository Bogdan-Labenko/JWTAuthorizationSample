using DotNetExam.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DotNetExam.MediatR.Queries
{
    public record GetUserByEmailQuery(string email) : IRequest<User?>;
}
