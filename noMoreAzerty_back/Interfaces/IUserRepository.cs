﻿using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task SaveChangesAsync();
    }
}
