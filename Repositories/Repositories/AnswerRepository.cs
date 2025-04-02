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
    }
}
