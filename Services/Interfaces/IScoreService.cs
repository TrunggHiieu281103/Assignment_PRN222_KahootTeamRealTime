using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IScoreService
    {
        Task<Score> GetUserScore(Guid userId, Guid roomId);
        Task<Score> AddUserScore(Guid userId, Guid roomId);
        Task<List<Score>> GetListScoreInRoom(Guid roomId);
    }
}
