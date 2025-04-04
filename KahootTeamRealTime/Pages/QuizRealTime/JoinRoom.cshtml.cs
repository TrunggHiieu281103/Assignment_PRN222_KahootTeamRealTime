using KahootTeamRealTime.HubSginalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Services.Interfaces;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class JoinRoomModel : PageModel
    {
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IHubContext<QuizHub> _hubContext;

        public JoinRoomModel(IRoomService roomService, IUserService userService, IHubContext<QuizHub> hubContext)
        {
            _roomService = roomService;
            _userService = userService;
            _hubContext = hubContext;
        }

        [BindProperty] public int RoomCode { get; set; }
        [BindProperty] public string Username { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var room = await _roomService.GetRoomByCodeAsync(RoomCode);
            if (room == null)
            {
                ErrorMessage = "Room không tồn tại!";
                return Page();
            }

            var user = await _userService.GetUserByUserName(Username);
            if (user == null)
            {
                user = await _userService.CreateUserAsync(Username);
            }

            bool isUserInRoom = await _roomService.IsUserInRoom(user.Id, room.Id);
            if (!isUserInRoom)
            {
                await _roomService.AddUserToRoom(user.Id, room.Id);
                await _hubContext.Clients.All.SendAsync("ReceiveUserJoined", RoomCode);
            }

            return RedirectToPage("RoomUsers", new { roomCode = RoomCode, username = Username });
        }
    }
}
