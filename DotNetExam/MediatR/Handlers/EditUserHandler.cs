using DotNetExam.MediatR.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotNetExam.MediatR.Handlers
{
    public class EditUserHandler : IRequestHandler<EditUserCommand>
    {
        private readonly AppDbContext _context;
        public EditUserHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.user.Id, cancellationToken);
            if(user != null) 
            {
                user.UserName = request.user.UserName;
                user.Email = request.user.Email;
                user.RefreshToken = request.user.RefreshToken;
                user.PasswordHash = request.user.PasswordHash;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
