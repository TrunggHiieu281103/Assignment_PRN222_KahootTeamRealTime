using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class RoomQuestion
{
    public Guid Id { get; set; }

    public Guid RoomId { get; set; }

    public Guid QuestionId { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
