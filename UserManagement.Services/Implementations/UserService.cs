using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UserManagement.Data;
using UserManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagement.Services.Domain;

namespace UserManagement.Services.Domain;

public class UserService(IDataContext dataContext, ILogger<UserService> logger) : IUserService
{

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public async Task<IReadOnlyList<User>> FilterByActiveAsync(bool isActive, CancellationToken ct = default)
    {
        logger?.LogDebug("Filtering users by IsActive={IsActive}", isActive);

        return await dataContext
            .GetAll<User>()
            .Where(u => u.IsActive == isActive)
            .AsNoTracking()
            .ToListAsync(ct);
    }
    public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct)
    {
        return await dataContext.GetAll<User>()
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<User?> GetByIdAsync(long id, CancellationToken ct = default) =>
       await dataContext.GetAll<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task CreateAsync(User user, CancellationToken ct = default)
    {
        dataContext.Create(user);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        dataContext.Update(user);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await dataContext.GetAll<User>().FirstOrDefaultAsync(u => u.Id == id, ct);
        if (entity is not null) dataContext.Delete(entity); // sync
    }

}
