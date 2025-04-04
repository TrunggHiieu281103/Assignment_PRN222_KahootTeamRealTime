using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories.Models;
using Microsoft.AspNetCore.Http;
using Services.Services;
using Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using KahootTeamRealTime.HubSginalR;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class AnswerQuizModel : PageModel
    {
        private readonly IQuestionService _questionService;
        private readonly IRoomService _roomService;
        private readonly IUserService _userService;
        private readonly IScoreService _scoreService;
        private readonly IAnswerService _answerService;
        private readonly IHubContext<QuizHub> _hubContext;


        public AnswerQuizModel(IQuestionService questionService, IRoomService roomService, IHubContext<QuizHub> hubContext, IUserService userService, IScoreService scoreService, IAnswerService answerService)
        {
            _questionService = questionService;
            _roomService = roomService;
            _hubContext = hubContext;
            _userService = userService;
            _scoreService = scoreService;
            _answerService = answerService;
        }

        public string Question { get; set; } = "N/A";
        public List<(string Id, string Content)> Answers { get; set; } = new();
        public int QuestionIndex { get; set; }
        public int RoomCode { get; set; }
        public string Username { get; set; } = "N/A";
        public int TotalQuestions { get; set; } = 0;
        public int TimeLeft { get; set; } = 10; // 10 giây mỗi câu hỏi

        [BindProperty]
        public string SelectedAnswer { get; set; }

        public async Task<IActionResult> OnGetAsync(int roomCode , string username, int questionIndex = 0)
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

        public async Task<IActionResult> OnPostAsync(int roomCode, string username, int questionIndex, Guid? SelectedAnswer)
        {
            var room = await _roomService.GetRoomByCodeAsync(roomCode);
            if (room == null)
            {
                return RedirectToPage("/Error");
            }

            var roomQuestions = await _roomService.GetRoomQuestion(room.Id);
            if (questionIndex >= roomQuestions.Count)
            {
                return RedirectToPage("/QuizRealTime/QuestionScores", new { roomCode });
            }

            var user = await _userService.GetUserByUserName(username);
            if (user == null)
            {
                return RedirectToPage("/Error");
            }

            // **Lấy hoặc tạo điểm số**
            var userScore = await _scoreService.GetUserScore(user.Id, room.Id);
            if (userScore == null)
            {
                userScore = await _scoreService.AddUserScore(user.Id, room.Id);
            }
            else if (questionIndex == 0)
            {
                userScore.TotalPoints = 0; // Reset điểm nếu người chơi bắt đầu lại từ đầu
            }

            // **Kiểm tra câu trả lời đúng**
            bool isCorrect = false;
            if (SelectedAnswer.HasValue)
            {
                var answer = await _answerService.GetAnswerByIdAsync(SelectedAnswer.Value);
                isCorrect = answer != null && answer.IsCorrect;
            }

            if (isCorrect)
            {
                userScore.TotalPoints += 100;
            }

            // **Nếu hết câu hỏi, chuyển trang tổng kết**
            if (questionIndex >= roomQuestions.Count - 1)
            {
                await _roomService.RemoveUserFromRoom(user.Id, room.Id);
                await _hubContext.Clients.All.SendAsync("ReceiveUserFinished", roomCode);
                await _hubContext.Clients.All.SendAsync("ReceiveUserJoined", roomCode);

                return RedirectToPage("/QuizRealTime/QuestionScores", new { roomCode });
            }

            return RedirectToPage("/QuizRealTime/AnswerQuiz", new { roomCode, username, questionIndex = questionIndex + 1 });
        }

    }
}
