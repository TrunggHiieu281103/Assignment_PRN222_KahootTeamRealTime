using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Models;

namespace Services.Interfaces
{
    public interface IAnswerService
    {
        Task<Answer> CreateAnswerAsync(Guid questionId, string content, bool isCorrect);
        Task<IEnumerable<Answer>> GetAnswersForQuestionAsync(Guid questionId);
    }
}
