using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserManagement.Models;
using UserManagement.Services.Domain;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController(IUserService userService, ILogger<UsersController> logger) : Controller
{

    [HttpGet("")]
    public async Task<IActionResult> List(string? status, CancellationToken ct = default)
    {
        var filter = status?.ToLowerInvariant();
        logger.LogDebug("List requested with status='{Status}'", filter);

        var users = filter switch
        {
            "active" => await userService.FilterByActiveAsync(true, ct),
            "inactive" => await userService.FilterByActiveAsync(false, ct),
            _ => await userService.GetAllAsync(ct)
        };

        logger.LogInformation("List returning {Count} users (filter='{Status}')", users.Count, filter ?? "all");

        var model = new UserListViewModel
        {
            Items = users.Select(u => new UserListItemViewModel
            {
                Id = u.Id,
                Forename = u.Forename,
                Surname = u.Surname,
                Email = u.Email,
                IsActive = u.IsActive,
                DateOfBirth = u.DateOfBirth
            }).ToList()
        };

        return View(model);
    }


    [HttpGet("create")]
    public IActionResult Create()
    {
        logger.LogDebug("Create (GET) requested");
        return View(new UserEditViewModel { IsActive = true });
    }

    // POST /users/create
    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserEditViewModel vm, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
        {
            logger.LogWarning("Create (POST) validation failed with {ErrorCount} errors",
                ModelState.Values.SelectMany(v => v.Errors).Count());
            return View(vm);
        }

        var entity = new User
        {
            Forename = vm.Forename,
            Surname = vm.Surname,
            Email = vm.Email,
            IsActive = vm.IsActive,
            DateOfBirth = vm.DateOfBirth
        };

        await userService.CreateAsync(entity, ct);
        logger.LogInformation("User created: {Email}", entity.Email);

        return RedirectToAction(nameof(List));
    }


    [HttpGet("view/{id:long}")]
    public async Task<IActionResult> ViewUser(long id, CancellationToken ct = default)
    {
        using var _ = logger.BeginScope(new { UserId = id });

        var user = await userService.GetByIdAsync(id, ct);
        if (user is null)
        {
            logger.LogWarning("View requested for missing user");
            return NotFound();
        }

        logger.LogDebug("View requested for user {Email}", user.Email);

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        return View("View", vm);
    }


    [HttpGet("edit/{id:long}")]
    public async Task<IActionResult> Edit(long id, CancellationToken ct = default)
    {
        using var _ = logger.BeginScope(new { UserId = id });

        var user = await userService.GetByIdAsync(id, ct);
        if (user is null)
        {
            logger.LogWarning("Edit (GET) requested for missing user");
            return NotFound();
        }

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        logger.LogDebug("Edit (GET) loaded for user {Email}", user.Email);
        return View(vm);
    }


    [HttpPost("edit/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, UserEditViewModel vm, CancellationToken ct = default)
    {
        using var _ = logger.BeginScope(new { UserId = id });

        if (id != vm.Id)
        {
            logger.LogWarning("Edit (POST) id mismatch: route={RouteId}, model={ModelId}", id, vm.Id);
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            logger.LogWarning("Edit (POST) validation failed with {ErrorCount} errors",
                ModelState.Values.SelectMany(v => v.Errors).Count());
            return View(vm);
        }

        var existing = await userService.GetByIdAsync(id, ct);
        if (existing is null)
        {
            logger.LogWarning("Edit (POST) for missing user");
            return NotFound();
        }

        existing.Forename = vm.Forename;
        existing.Surname = vm.Surname;
        existing.Email = vm.Email;
        existing.IsActive = vm.IsActive;
        existing.DateOfBirth = vm.DateOfBirth;

        await userService.UpdateAsync(existing, ct);
        logger.LogInformation("User updated: {Email}", existing.Email);

        return RedirectToAction(nameof(List));
    }


    [HttpGet("delete/{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct = default)
    {
        using var _ = logger.BeginScope(new { UserId = id });

        var user = await userService.GetByIdAsync(id, ct);
        if (user is null)
        {
            logger.LogWarning("Delete (GET) requested for missing user");
            return NotFound();
        }

        var vm = new UserEditViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        logger.LogDebug("Delete (GET) confirm for user {Email}", user.Email);
        return View(vm);
    }


    [HttpPost("delete/{id:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id, CancellationToken ct = default)
    {
        using var _ = logger.BeginScope(new { UserId = id });

        await userService.DeleteAsync(id, ct);
        logger.LogInformation("User deleted");

        return RedirectToAction(nameof(List));
    }
}
