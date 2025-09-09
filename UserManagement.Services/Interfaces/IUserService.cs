using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain;

public interface IUserService
{
    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<User>> FilterByActiveAsync(bool isActive, CancellationToken ct = default);

    Task<User?> GetByIdAsync(long id, CancellationToken ct = default);
    Task CreateAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task DeleteAsync(long id, CancellationToken ct = default);
}
