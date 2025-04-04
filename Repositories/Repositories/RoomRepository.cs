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
    public class RoomRepository : GenericRepository<Room>
    {
        public RoomRepository(RealtimeQuizDbContext context, ILogger logger)
        : base(context, logger)
        {
        }
        public Room GetRoomByCode(int roomCode)
        {
            return _dbSet.FirstOrDefault(r => r.RoomCode == roomCode);
        }

        public Room GetRoomWithDetails(Guid roomId)
        {
            return _context.Rooms
                .Include(r => r.UserRooms)
                .ThenInclude(ur => ur.User)
                .FirstOrDefault(r => r.Id == roomId);
        }

        public List<Question> GetRoomQuestion(Guid roomId)
        {
            return _context.RoomQuestions
                .Where(rq => rq.RoomId == roomId)
                .Select(rq => rq.Question)
                .ToList();
        }

        public async Task<bool> RemoveUserFromRoom(Guid userId, Guid roomId)
        {
            var userRoom = await _context.UserRooms
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoomId == roomId);

            if (userRoom != null)
            {
                _context.UserRooms.Remove(userRoom);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        // Kiểm tra xem người dùng có trong phòng không
        public async Task<bool> IsUserInRoom(Guid userId, Guid roomId)
        {
            return await _context.UserRooms
                .AnyAsync(ur => ur.UserId == userId && ur.RoomId == roomId);
        }
        // Thêm người dùng vào phòng
        public async Task<bool> AddUserToRoom(Guid userId, Guid roomId)
        {
            if (await IsUserInRoom(userId, roomId))
                return false;

            var userRoom = new UserRoom { UserId = userId, RoomId = roomId };
            _context.UserRooms.Add(userRoom);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<(string Name, int Points)>> GetUserScoresByRoomCode(int roomCode)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (room == null) return new List<(string, int)>();

            var userScores = await _context.Scores
                .Where(s => s.RoomId == room.Id)
                .OrderByDescending(s => s.TotalPoints)
                .Join(
                    _context.Users,
                    score => score.UserId,
                    user => user.Id,
                    (score, user) => new { user.Username, score.TotalPoints }
                )
                .ToListAsync();

            return userScores.Select(us => (us.Username, us.TotalPoints ?? 0)).ToList();
        }

        public async Task<List<string>> GetUsernamesByRoomCode(int roomCode)
        {
            return await _context.UserRooms
                .Where(ur => ur.Room.RoomCode == roomCode)
                .Select(ur => ur.User.Username)
                .ToListAsync();
        }
    }
}
