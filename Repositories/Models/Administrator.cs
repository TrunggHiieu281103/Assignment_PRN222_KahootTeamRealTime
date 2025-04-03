using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Administrator
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public bool IsActive { get; set; }

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;
}
