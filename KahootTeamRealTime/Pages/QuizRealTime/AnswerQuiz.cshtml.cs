using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories.Models;
using Microsoft.AspNetCore.Http;
using Services.Services;
using Services.Interfaces;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class AnswerQuizModel : PageModel
    {
        private readonly IQuestionService _questionService;
        private readonly IRoomService _roomService;

        public AnswerQuizModel(IQuestionService questionService, IRoomService roomService)
        {
            _questionService = questionService;
            _roomService = roomService;
        }

        public string Question { get; set; }
        public List<(string Id, string Content)> Answers { get; set; } = new();
        public int QuestionIndex { get; set; }
        public int RoomCode { get; set; }
        public string Username { get; set; }
        public int TotalQuestions { get; set; }

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

        public async Task<IActionResult> OnPostAsync(int roomCode, string username, int questionIndex)
        {
            var room = await _roomService.GetRoomByCodeAsync(roomCode);
            if (room == null)
            {
                return RedirectToPage("/Error");
            }

            var roomQuestions = await _questionService.GetQuestionsByRoomCodeAsync(roomCode);
            var questionList = roomQuestions.ToList();

            // Kiểm tra đáp án đúng
            bool isCorrect = false;
            if (questionIndex < questionList.Count)
            {
                var currentQuestion = questionList[questionIndex];
                isCorrect = currentQuestion.Answers.Any(a => a.Id.ToString() == SelectedAnswer && a.IsCorrect);
            }

            return new JsonResult(new
            {
                isCorrect
            });
        }
    }
}
