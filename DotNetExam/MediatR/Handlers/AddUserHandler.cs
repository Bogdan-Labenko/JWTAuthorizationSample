using DotNetExam.MediatR.Commands;
using MediatR;

namespace DotNetExam.MediatR.Handlers
{
    public class AddUserHandler : IRequestHandler<AddUserCommand>
    {
        private readonly AppDbContext _context;
        public AddUserHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(request.user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
