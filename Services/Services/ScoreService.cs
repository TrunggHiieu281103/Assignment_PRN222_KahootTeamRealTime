using Repositories.Infrastructures;
using Repositories.Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class ScoreService : IScoreService
    {
        private readonly UnitOfWork _unitOfWork;

        public ScoreService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Score> AddUserScore(Guid userId, Guid roomId)
        {
            // Nếu user chưa có điểm, tạo mới
            var userScore = new Score
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoomId = roomId,
                TotalPoints = 0
            };
            var newUserScore = _unitOfWork.ScoreRepository.AddEntity(userScore);
            await _unitOfWork.CompleteAsync();
            return newUserScore;
        }

        public async Task<List<Score>> GetListScoreInRoom(Guid roomId)
        {
            return _unitOfWork.ScoreRepository.GetListScoreInRoom(roomId);
        }

        public async Task<Score> GetUserScore(Guid userId, Guid roomId)
        {
            return _unitOfWork.ScoreRepository.GetScore(userId, roomId);
        }
    }
}
