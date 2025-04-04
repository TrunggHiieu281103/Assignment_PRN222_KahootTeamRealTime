using KahootTeamRealTime.HubSginalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Services.Interfaces;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class RoomUsersModel : PageModel
    {

        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IHubContext<QuizHub> _hubContext;

        public RoomUsersModel(IRoomService roomService, IHubContext<QuizHub> hubContext, IUserService userService)
        {
            _roomService = roomService;
            _hubContext = hubContext;
            _userService = userService;
        }

        public int RoomCode { get; set; }
        public string Username { get; set; }
        public List<string> Users { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int roomCode, string username)
        {
            RoomCode = roomCode;
            Username = username;
            Users = await _roomService.GetUsernamesByRoomCode(roomCode);

            if (Users.Count == 0)
            {
                return RedirectToPage("/Error");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostLeaveRoomAsync(int roomCode, string username)
        {
            var user = await _userService.GetUserByUserName(username);
            var room = await _roomService.GetRoomByCodeAsync(roomCode);
            if (user == null || room == null)
            {
                return RedirectToPage("/Error");
            }
            bool removed = await _roomService.RemoveUserFromRoom(user.Id, room.Id);
            if (removed)
            {
                // Gửi sự kiện SignalR để thông báo người dùng rời phòng
                await _hubContext.Clients.All.SendAsync("ReceiveUserJoined", roomCode);
            }

            return RedirectToPage("JoinRoom");
        }
    }
}
