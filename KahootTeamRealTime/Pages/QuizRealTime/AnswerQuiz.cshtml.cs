using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories.Models;
using Microsoft.AspNetCore.Http;
using Services.Services;
using Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class AnswerQuizModel : PageModel
    {
        private readonly IQuestionService _questionService;
        private readonly IRoomService _roomService;
        private readonly RealtimeQuizDbContext _context;

        public AnswerQuizModel(IQuestionService questionService, IRoomService roomService, RealtimeQuizDbContext context)
        {
            _questionService = questionService;
            _roomService = roomService;
            _context = context;
        }

        public string Question { get; set; }
        public List<(string Id, string Content)> Answers { get; set; } = new();
        public int QuestionIndex { get; set; }
        public int RoomCode { get; set; }
        public string Username { get; set; }
        public int TotalQuestions { get; set; }
        public int TimeLeft { get; set; } = 10; // 10 giây mỗi câu hỏi

        [BindProperty]
        public string SelectedAnswer { get; set; }

        public async Task<IActionResult> OnGetAsync(int roomCode = 100000, string username = "Amoi", int questionIndex = 0)
        {
            var room = await _roomService.GetRoomByCodeAsync(roomCode);
            if (room == null)
            {
                return RedirectToPage("/Error");
            }

            RoomCode = roomCode;
            Username = username;

            var roomQuestions = await _questionService.GetQuestionsByRoomCodeAsync(roomCode);
            var questionList = roomQuestions.ToList();
            TotalQuestions = questionList.Count;

            if (questionList.Count == 0 || questionIndex >= questionList.Count)
            {
                return RedirectToPage("/QuestionScores"); // Nếu hết câu hỏi thì kết thúc quiz
            }

            var currentQuestion = questionList[questionIndex];

            QuestionIndex = questionIndex;
            Question = currentQuestion.Content;
            Answers = currentQuestion.Answers.Select(a => (a.Id.ToString(), a.Content)).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int roomCode, string username, int questionIndex, Guid? selectedAnswerId)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomCode == roomCode);
            if (room == null)
            {
                return RedirectToPage("/Error");
            }

            var roomQuestions = await _context.RoomQuestions
                .Where(rq => rq.RoomId == room.Id)
                .Select(rq => rq.Question)
                .ToListAsync();

            if (questionIndex >= roomQuestions.Count)
            {
                return RedirectToPage("/QuizRealTime/QuestionScores", new { roomCode });
            }

            var currentQuestion = roomQuestions[questionIndex];

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return RedirectToPage("/Error");
            }

            // **Xử lý khi người chơi không chọn câu trả lời**
            bool isCorrect = false;
            if (selectedAnswerId.HasValue)
            {
                isCorrect = currentQuestion.Answers.Any(a => a.Id == selectedAnswerId.Value && a.IsCorrect);
            }

            // **Lưu UserAnswer với AnswerId = null nếu không chọn**
            var userAnswer = new UserAnswer
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoomId = room.Id,
                QuestionId = currentQuestion.Id,
                AnswerId = selectedAnswerId, // **Có thể null**
                AnsweredAt = DateTime.UtcNow
            };
            _context.UserAnswers.Add(userAnswer);

            // Cập nhật điểm trong bảng Score
            var userScore = await _context.Scores
                .FirstOrDefaultAsync(s => s.UserId == user.Id && s.RoomId == room.Id);

            if (userScore == null)
            {
                userScore = new Score
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    RoomId = room.Id,
                    TotalPoints = 0
                };
                _context.Scores.Add(userScore);
            }

            if (isCorrect)
            {
                userScore.TotalPoints += 100;
            }

            await _context.SaveChangesAsync();

            // Nếu hết câu hỏi, chuyển sang trang tổng kết điểm
            if (questionIndex >= roomQuestions.Count - 1)
            {
                return RedirectToPage("/QuizRealTime/QuestionScores", new { roomCode });
            }

            // Chuyển đến câu hỏi tiếp theo
            return RedirectToPage("/QuizRealTime/AnswerQuiz", new { roomCode, username, questionIndex = questionIndex + 1 });
        }

    }
}
