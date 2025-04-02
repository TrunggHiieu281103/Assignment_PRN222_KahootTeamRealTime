using KahootTeamRealTimeAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Services;

namespace KahootTeamRealTimeAdmin.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IRoomService _roomService;

        public QuestionsController(IQuestionService questionService, IRoomService roomService)
        {
            _questionService = questionService;
            _roomService = roomService;
        }

        // GET: Questions
        public async Task<IActionResult> Index(Guid? roomId)
        {
            // Store roomId in ViewBag for the view
            ViewBag.RoomId = roomId;

            if (roomId.HasValue)
            {
                // Get the room to display its name
                var room = await _roomService.GetRoomByIdAsync(roomId.Value);
                ViewBag.RoomName = room?.Name ?? "Unknown Room";

                // Get questions for this specific room
                var questions = await _questionService.GetQuestionsForRoomAsync(roomId.Value);
                return View(questions);
            }
            else
            {
                // Get all questions if no roomId is specified
                var questions = await _questionService.GetQuestionsWithAnswersAsync();
                return View(questions);
            }
        }

        // GET: Questions/Create
        public IActionResult Create(Guid? roomId)
        {
            ViewBag.RoomId = roomId;
            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(QuestionViewModel model, Guid? roomId)
        {
            if (ModelState.IsValid)
            {
                await _questionService.CreateQuestionAsync(model.Content, roomId);

                if (roomId.HasValue)
                {
                    return RedirectToAction(nameof(Index), new { roomId });
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.RoomId = roomId;
            return View(model);
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(Guid id, Guid? roomId)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            ViewBag.RoomId = roomId;
            return View(question);
        }

        // GET: Questions/AddExisting
        public async Task<IActionResult> AddExisting(Guid roomId)
        {
            // Get all questions not already in this room
            var roomQuestions = await _questionService.GetQuestionsForRoomAsync(roomId);
            var allQuestions = await _questionService.GetQuestionsWithAnswersAsync();

            var availableQuestions = allQuestions.Where(q =>
                !roomQuestions.Any(rq => rq.Id == q.Id)).ToList();

            ViewBag.RoomId = roomId;
            return View(availableQuestions);
        }

        // POST: Questions/AddToRoom
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToRoom(Guid roomId, Guid questionId)
        {
            await _questionService.AddQuestionToRoomAsync(roomId, questionId);
            return RedirectToAction(nameof(Index), new { roomId });
        }
    }
}
