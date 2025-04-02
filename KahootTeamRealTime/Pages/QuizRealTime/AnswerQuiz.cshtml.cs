using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories.Models;
using Microsoft.AspNetCore.Http;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class AnswerQuizModel : PageModel
    {
        private readonly RealtimeQuizDbContext _context;

        public AnswerQuizModel(RealtimeQuizDbContext context)
        {
            _context = context;
        }

        public string Question { get; set; }
        public List<(string Id, string Content)> Answers { get; set; } = new();
        public int QuestionIndex { get; set; } // Chỉ mục câu hỏi hiện tại
        public int RoomCode { get; set; } 
        public string Username { get; set; }

        [BindProperty]
        public string SelectedAnswer { get; set; }

        public void OnGet(int roomCode = 100000, string username = "Amoi")
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomCode == roomCode);
            if (room == null)
            {
                return;
            }

            RoomCode = roomCode;
            Username = username;

            // Lấy chỉ mục câu hỏi từ Session (mặc định là 0 nếu chưa có)
            QuestionIndex = HttpContext.Session.GetInt32($"QuestionIndex_{roomCode}_{username}") ?? 0;

            var roomQuestions = _context.RoomQuestions
                .Where(rq => rq.RoomId == room.Id)
                .OrderBy(rq => rq.QuestionId)
                .ToList();

            if (roomQuestions.Any() && QuestionIndex < roomQuestions.Count)
            {
                var currentQuestion = roomQuestions[QuestionIndex]; // Lấy câu hỏi hiện tại
                Question = _context.Questions
                    .Where(q => q.Id == currentQuestion.QuestionId)
                    .Select(q => q.Content)
                    .FirstOrDefault();

                Answers = _context.Answers
                    .Where(a => a.QuestionId == currentQuestion.QuestionId)
                    .Select(a => new { a.Id, a.Content })
                    .ToList()
                    .Select(a => (a.Id.ToString(), a.Content))
                    .ToList();
            }
        }

        public IActionResult OnPost(int roomCode, string username)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomCode == roomCode);
            if (room == null)
            {
                return NotFound();
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            var roomQuestions = _context.RoomQuestions
                .Where(rq => rq.RoomId == room.Id)
                .OrderBy(rq => rq.QuestionId)
                .ToList();

            QuestionIndex = HttpContext.Session.GetInt32($"QuestionIndex_{roomCode}_{Username}") ?? 0;

            if (QuestionIndex >= roomQuestions.Count)
            {
                return RedirectToPage("QuestionScores"); // Nếu hết câu hỏi, chuyển trang điểm
            }

            var currentQuestion = roomQuestions[QuestionIndex];

            var userAnswer = new UserAnswer
            {
                UserId = user.Id,
                RoomId = room.Id,
                QuestionId = currentQuestion.QuestionId,
                AnswerId = Guid.TryParse(SelectedAnswer, out Guid answerId) ? answerId : null,
                AnsweredAt = DateTime.Now
            };

            _context.UserAnswers.Add(userAnswer);
            _context.SaveChanges();

            // Tăng chỉ mục câu hỏi và lưu vào Session
            HttpContext.Session.SetInt32($"QuestionIndex_{roomCode}_{Username}", QuestionIndex + 1);

            return RedirectToPage("AnswerQuiz", new { roomCode, username = Username });
        }
    }
}
