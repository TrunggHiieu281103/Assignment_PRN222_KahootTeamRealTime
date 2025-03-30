using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Score? Score { get; set; }

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

    public virtual ICollection<UserRoom> UserRooms { get; set; } = new List<UserRoom>();
}
