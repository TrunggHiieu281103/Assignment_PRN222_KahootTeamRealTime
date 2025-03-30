using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Room
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int RoomCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<UserRoom> UserRooms { get; set; } = new List<UserRoom>();
}
