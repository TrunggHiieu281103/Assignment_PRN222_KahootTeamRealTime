using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class UserRoom
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid RoomId { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
