using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Room
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int RoomCode { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<RoomQuestion> RoomQuestions { get; set; } = new List<RoomQuestion>();

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();

    public virtual ICollection<UserRoom> UserRooms { get; set; } = new List<UserRoom>();
}
