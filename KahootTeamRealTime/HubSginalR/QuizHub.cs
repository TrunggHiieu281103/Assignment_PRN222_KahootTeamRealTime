using Microsoft.AspNetCore.SignalR;
using Repositories.Models;

namespace KahootTeamRealTime.HubSginalR
{
    public class QuizHub:Hub
    {
        private readonly RealtimeQuizDbContext _context;

        public QuizHub(RealtimeQuizDbContext context)
        {
            _context = context;
        }

        public async Task JoinRoom(int roomCode, string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode.ToString());

            var room = _context.Rooms.FirstOrDefault(r => r.RoomCode == roomCode);
            if (room == null) return;

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                user = new User { Username = username };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var userRoom = _context.UserRooms.FirstOrDefault(ur => ur.UserId == user.Id && ur.RoomId == room.Id);
            if (userRoom == null)
            {
                _context.UserRooms.Add(new UserRoom { UserId = user.Id, RoomId = room.Id });
                await _context.SaveChangesAsync();
            }

            var usersInRoom = _context.UserRooms
                .Where(ur => ur.RoomId == room.Id)
                .Select(ur => ur.User.Username)
                .ToList();

            await Clients.Group(roomCode.ToString()).SendAsync("UpdateRoomUsers", usersInRoom);
        }

        public async Task LeaveRoom(int roomCode, string username)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode.ToString());

            var room = _context.Rooms.FirstOrDefault(r => r.RoomCode == roomCode);
            if (room == null) return;

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return;

            var userRoom = _context.UserRooms.FirstOrDefault(ur => ur.UserId == user.Id && ur.RoomId == room.Id);
            if (userRoom != null)
            {
                _context.UserRooms.Remove(userRoom);
                await _context.SaveChangesAsync();
            }

            var usersInRoom = _context.UserRooms
                .Where(ur => ur.RoomId == room.Id)
                .Select(ur => ur.User.Username)
                .ToList();

            await Clients.Group(roomCode.ToString()).SendAsync("UpdateRoomUsers", usersInRoom);
        }
    }
}
