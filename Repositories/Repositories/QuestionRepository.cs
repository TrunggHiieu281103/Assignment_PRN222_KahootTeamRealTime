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
    public class QuestionRepository : GenericRepository<Question>
    {
        public QuestionRepository(RealtimeQuizDbContext context, ILogger logger)
        : base(context, logger)
        {
        }
        public IEnumerable<Question> GetQuestionsWithAnswers()
        {
            return _context.Questions
                .Include(q => q.Answers)
                .ToList();
        }

        public Question GetQuestionWithAnswers(Guid questionId)
        {
            return _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefault(q => q.Id == questionId);
        }


        public IEnumerable<Question> GetQuestionsForRoom(Guid roomId)
        {
            return _context.RoomQuestions
                .Where(rq => rq.RoomId == roomId)
                .Include(rq => rq.Question)
                .ThenInclude(q => q.Answers)
                .Select(rq => rq.Question)
                .ToList();
        }

        public void AddQuestionToRoom(Guid roomId, Guid questionId)
        {
            var roomQuestion = new RoomQuestion
            {
                RoomId = roomId,
                QuestionId = questionId
            };

            _context.RoomQuestions.Add(roomQuestion);
        }
    }
}
