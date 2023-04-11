using DotNetExam.Models;
using MediatR;

namespace DotNetExam.MediatR.Commands
{
    public record AddUserCommand(User user) : IRequest;
}
