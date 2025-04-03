using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Models;

namespace Services.Interfaces
{
    public interface IRoomService
    {
        Task<Room> CreateRoomAsync(string name);
        Task<Room> GetRoomByIdAsync(Guid id);
        Task<Room> GetRoomByCodeAsync(int roomCode);
        Task<IEnumerable<Room>> GetAllRoomsAsync();

        Task<bool> ToggleRoomActiveStatusAsync(Guid roomId);
    }
}
