using KahootTeamRealTime.HubSginalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class JoinRoomModel : PageModel
    {
        private readonly RealtimeQuizDbContext _context;
        private readonly IHubContext<QuizHub> _hubContext;

        public JoinRoomModel(RealtimeQuizDbContext context, IHubContext<QuizHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [BindProperty] public int RoomCode { get; set; }
        [BindProperty] public string Username { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomCode == RoomCode);
            if (room == null)
            {
                ErrorMessage = "Room not found!";
                return Page();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == Username);
            if (user == null)
            {
                user = new User { Username = Username };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var existingUserRoom = _context.UserRooms
                .FirstOrDefault(ur => ur.UserId == user.Id && ur.RoomId == room.Id);
            if (existingUserRoom == null)
            {
                _context.UserRooms.Add(new UserRoom { UserId = user.Id, RoomId = room.Id });
                await _context.SaveChangesAsync();
            }

            // Gửi sự kiện cập nhật danh sách người dùng trong phòng qua SignalR
            var usersInRoom = _context.UserRooms
                .Where(ur => ur.RoomId == room.Id)
                .Select(ur => ur.User.Username)
                .ToList();

            await _hubContext.Clients.Group(RoomCode.ToString()).SendAsync("UpdateRoomUsers", usersInRoom);

            return RedirectToPage("RoomUsers", new { roomCode = RoomCode, username = Username });
        }
    }
}
