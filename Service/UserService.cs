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
            return await _context.AppUsers
                .Include(u => u.Province)
                .Include(u => u.District)
                .Include(u => u.Bills)
                .ToListAsync();
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.AppUsers
                .Include(u => u.Province)
                .Include(u => u.District)
                .Include(u => u.Bills)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }
        public async Task AddAsync(User user)
        {
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(User user)
        {
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null) return;
            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
