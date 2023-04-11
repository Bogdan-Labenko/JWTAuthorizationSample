using DotNetExam.MediatR.Queries;
using DotNetExam.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DotNetExam.MediatR.Handlers
{
    public class GetUserByEmailHandler : IRequestHandler<GetUserByEmailQuery, User?>
    {
        private readonly AppDbContext _context;
        public GetUserByEmailHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<User?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == request.email, cancellationToken);
        }
    }
}
