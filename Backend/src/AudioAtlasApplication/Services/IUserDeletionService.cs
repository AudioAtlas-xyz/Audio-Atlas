using System;

namespace AudioAtlasApplication.Services;

public interface IUserDeletionService
{
    Task<bool> DeleteUserAsync(Guid userId);
}