using DotNetExam.MediatR.Queries;
using DotNetExam.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DotNetExam.MediatR.Handlers
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, List<User>>
    {
        private readonly AppDbContext _context;
        public GetAllUsersHandler(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            return await _context.Users.ToListAsync(cancellationToken);
        }
    }
}
