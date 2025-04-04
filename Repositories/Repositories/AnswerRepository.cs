using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Infrastructures;
using Repositories.Models;

namespace Repositories.Repositories
{
    public class AnswerRepository : GenericRepository<Answer>
    {
        public AnswerRepository(RealtimeQuizDbContext context, ILogger logger)
        : base(context, logger)
        {
        }

        public IEnumerable<Answer> GetAnswersForQuestion(Guid questionId)
        {
            return _dbSet.Where(a => a.QuestionId == questionId).ToList();
        }

        public IEnumerable<Answer> GetAnswersByQuestionId(Guid questionId)
        {
            return _dbSet
                .Where(a => a.QuestionId == questionId)
                .ToList();
        }

        public void RemoveAnswersByQuestionId(Guid questionId)
        {
            var answers = GetAnswersByQuestionId(questionId);
            foreach (var answer in answers)
            {
                _dbSet.Remove(answer);
            }
        }

        public Answer GetAnswerWithRelationships(Guid id)
        {
            return _context.Answers
                .Include(a => a.Question)
                .Include(a => a.UserAnswers)
                .FirstOrDefault(a => a.Id == id);
        }

        public void RemoveUserAnswersByAnswerId(Guid answerId)
        {
            var userAnswers = _context.UserAnswers
                .Where(ua => ua.AnswerId == answerId)
                .ToList();

            foreach (var userAnswer in userAnswers)
            {
                // Set the AnswerId to null rather than deleting the record
                userAnswer.AnswerId = null;
                _context.UserAnswers.Update(userAnswer);
            }
        }
    }
}
