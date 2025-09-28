using BanhMyIT.Interface;
using BanhMyIT.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BanhMyIT.Service
{
    public class UserService : IUserService
    {
        private readonly BanhMyITDbContext _context;
        public UserService(BanhMyITDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.Include(u => u.Bills).ToListAsync();
        }
        public async Task<User> GetByIdAsync(int id)
        {
            return await _context.Users.Include(u => u.Bills).FirstOrDefaultAsync(u => u.UserID == id);
        }
        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}

