using System.Threading.Tasks;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);

        public IMessageRepository MessageRepository => new MessageRepository(_context, _mapper);

        public ILikesRepository LikesRepository => new LikesRepository(_context, _mapper);

        public async Task<bool> Complete()
        {
            // all changes tracked - use this to save all changes
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            // if it has any changes -- return true
            return _context.ChangeTracker.HasChanges();
        }
    }
}