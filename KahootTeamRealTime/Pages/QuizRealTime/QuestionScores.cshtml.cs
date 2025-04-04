using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;
using Services.Interfaces;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class QuestionScoresModel : PageModel
    {
        private readonly IRoomService _roomService;

        public QuestionScoresModel(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public List<(string Name, int Points)> UserPoints { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int roomCode)
        {
            UserPoints = await _roomService.GetUserScoresByRoomCode(roomCode);
            if (!UserPoints.Any())
            {
                return RedirectToPage("/Error");
            }

            return Page();
        }
    }
}
