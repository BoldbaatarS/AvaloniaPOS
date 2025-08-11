using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestaurantPOS.Models;


namespace RestaurantPOS.Data;

    public interface IUserRepository<T>
    {
        Task<List<UserModel>> GetAllAsync();
        Task<UserModel?> ValidateAsync(string pin);
        Task AddAsync(UserModel user);
        Task UpdateAsync(UserModel user);
        Task DeleteAsync(Guid id);
        Task<UserModel> GetByIdAsync(Guid id);
    }

