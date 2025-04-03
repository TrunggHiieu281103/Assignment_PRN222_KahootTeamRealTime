using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Repositories.Models;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class QuestionScoresModel : PageModel
    {
        private readonly RealtimeQuizDbContext _context;

        public QuestionScoresModel(RealtimeQuizDbContext context)
        {
            _context = context;
        }

        public List<(string Name, int Points)> UserPoints { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int roomCode)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (room == null)
            {
                return RedirectToPage("/Error");
            }

            var userScores = await _context.Scores
                .Where(s => s.RoomId == room.Id)
                .OrderByDescending(s => s.TotalPoints)
                .ToListAsync();

            foreach (var score in userScores)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == score.UserId);
                if (user != null)
                {
                    UserPoints.Add((user.Username, score.TotalPoints ?? 0));
                }
            }

            return Page();
        }
    }
}
