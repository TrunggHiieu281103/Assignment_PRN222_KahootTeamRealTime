using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Infrastructures;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class ScoreRepository : GenericRepository<Score>
    {
        public ScoreRepository(RealtimeQuizDbContext context, ILogger logger)
        : base(context, logger)
        {
        }

        public Score GetScore(Guid userId, Guid roomId)
        {
            return _context.Scores
                .FirstOrDefault(s => s.UserId == userId && s.RoomId == roomId);
        }

        public List<Score> GetListScoreInRoom(Guid roomId)
        {
            return _context.Scores
                .Where(s => s.RoomId == roomId)
                .OrderByDescending(s => s.TotalPoints)
                .ToList();
        }
    }
}
