using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Answer
{
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }

    public string Content { get; set; } = null!;

    public bool IsCorrect { get; set; }

    public virtual Question Question { get; set; } = null!;

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
