using KahootTeamRealTime.HubSginalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class RoomUsersModel : PageModel
    {

        private readonly RealtimeQuizDbContext _context;
        private readonly IHubContext<QuizHub> _hubContext;

        public RoomUsersModel(RealtimeQuizDbContext context, IHubContext<QuizHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public int RoomCode { get; set; }
        public string Username { get; set; }
        public List<string> Users { get; set; } = new();

        public void OnGet(int roomCode, string username)
        {
            RoomCode = roomCode;
            Username = username;

            var userIds = _context.UserRooms
                .Where(ur => ur.Room.RoomCode == roomCode)
                .Select(ur => ur.UserId)
                .ToList();

            Users = _context.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => u.Username)
                .ToList();
        }

        public async Task<IActionResult> OnPostLeaveRoomAsync(int roomCode, string username)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                var userRoom = _context.UserRooms.FirstOrDefault(ur => ur.UserId == user.Id && ur.Room.RoomCode == roomCode);
                if (userRoom != null)
                {
                    _context.UserRooms.Remove(userRoom);
                    await _context.SaveChangesAsync();

                    // Gửi sự kiện SignalR để thông báo rằng người dùng đã rời phòng
                    await _hubContext.Clients.All.SendAsync("ReceiveUserJoined", roomCode);

                }
            }

            return RedirectToPage("JoinRoom");
        }
    }
}
