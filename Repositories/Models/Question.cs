using System;
using System.Collections.Generic;

namespace Repositories.Models;

public partial class Question
{
    public Guid Id { get; set; }

    public string Content { get; set; } = null!;

    public virtual ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public virtual ICollection<RoomQuestion> RoomQuestions { get; set; } = new List<RoomQuestion>();

    public virtual ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
