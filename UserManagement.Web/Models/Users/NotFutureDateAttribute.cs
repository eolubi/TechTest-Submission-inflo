using System.ComponentModel.DataAnnotations;
using System;

namespace UserManagement.Web.Models.Users;

public sealed class NotFutureDateAttribute : ValidationAttribute
{
    public NotFutureDateAttribute() : base("{0} cannot be in the future.") { }

    public override bool IsValid(object? value)
        => value is not DateTime d || d.Date <= DateTime.Today;
}
