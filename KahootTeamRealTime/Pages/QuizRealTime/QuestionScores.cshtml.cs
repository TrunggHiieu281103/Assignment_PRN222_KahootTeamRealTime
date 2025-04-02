using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public void OnGet()
        {
            var userAnswers = _context.UserAnswers
                .GroupBy(ua => ua.UserId)
                .Select(group => new
                {
                    UserId = group.Key,
                    Points = group.Count(ua => ua.Answer.IsCorrect) // Tính điểm bằng số câu trả lời đúng
                })
                .ToList();

            foreach (var userAnswer in userAnswers)
            {
                var userName = _context.Users.FirstOrDefault(u => u.Id == userAnswer.UserId)?.Username;
                UserPoints.Add((userName ?? "Unknown", userAnswer.Points));
            }
        }
    }
}
