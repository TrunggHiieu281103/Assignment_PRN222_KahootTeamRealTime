using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Repositories.Infrastructures;
using Repositories.Models;

namespace Repositories.Repositories
{
    public class UserAnswerRepository : GenericRepository<UserAnswer>
    {
        public UserAnswerRepository(RealtimeQuizDbContext context, ILogger logger)
        : base(context, logger)
        {
        }

        public IEnumerable<UserAnswer> GetUserAnswersByQuestionId(Guid questionId)
        {
            return _dbSet
                .Where(ua => ua.QuestionId == questionId)
                .ToList();
        }

        public void RemoveUserAnswersByQuestionId(Guid questionId)
        {
            var userAnswers = GetUserAnswersByQuestionId(questionId);
            foreach (var userAnswer in userAnswers)
            {
                _dbSet.Remove(userAnswer);
            }
        }
    }
}
