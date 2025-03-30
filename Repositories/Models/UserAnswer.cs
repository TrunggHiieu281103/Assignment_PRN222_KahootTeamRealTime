using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class UserAnswer
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid QuestionId { get; set; }

    public Guid? AnswerId { get; set; }

    public DateTime? AnsweredAt { get; set; }

    public bool? IsTimeOut { get; set; }

    public virtual Answer? Answer { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
