using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<Administrator> Administrators { get; set; } = new List<Administrator>();
}
