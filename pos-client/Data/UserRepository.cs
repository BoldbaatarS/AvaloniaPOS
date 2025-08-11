using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantPOS.Data
{
    public class UserRepository : IUserRepository<UserModel>
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }
//dfgdfgdfgd
        public async Task<List<UserModel>> GetAllAsync()=>
            await _context.Users.ToListAsync();

        public async Task<UserModel> GetByIdAsync(Guid id) =>
            await _context.Users.FindAsync(id) ?? throw new KeyNotFoundException($"ID {id} хэрэглэгч олдсонгүй");

        public async Task AddAsync(UserModel entity)
        {
            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserModel entity)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        // PIN-аар хэрэглэгчийг шалгах
        public async Task<UserModel?> ValidateAsync(string pin)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Pin == pin);
        }

       
    }
}
