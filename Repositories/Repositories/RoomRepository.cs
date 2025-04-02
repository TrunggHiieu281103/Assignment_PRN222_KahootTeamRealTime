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

    }
}
