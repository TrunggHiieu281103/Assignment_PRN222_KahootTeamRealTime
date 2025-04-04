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
        Task<List<Question>> GetRoomQuestion(Guid roomId);
        Task<bool> RemoveUserFromRoom(Guid userId, Guid roomId);
        Task<bool> IsUserInRoom(Guid userId, Guid roomId);
        Task<bool> AddUserToRoom(Guid userId, Guid roomId);
        Task<List<(string Name, int Points)>> GetUserScoresByRoomCode(int roomCode);
        Task<List<string>> GetUsernamesByRoomCode(int roomCode);
    }
}
