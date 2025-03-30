using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Score
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public int? TotalPoints { get; set; }

    public virtual User User { get; set; } = null!;
}
