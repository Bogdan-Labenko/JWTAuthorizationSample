using DotNetExam.Models;
using MediatR;

namespace DotNetExam.MediatR.Queries
{
    public record GetAllUsersQuery : IRequest<List<User>>;
}
