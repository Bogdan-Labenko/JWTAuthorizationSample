using DotNetExam.Models;
using MediatR;

namespace DotNetExam.MediatR.Commands
{
    public record EditUserCommand(User user) : IRequest;
}
