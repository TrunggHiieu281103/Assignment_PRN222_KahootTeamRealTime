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

        // GET: Questions/Delete/{id}
        public async Task<IActionResult> Delete(Guid id, Guid? roomId)
        {
            var question = await _questionService.GetQuestionByIdAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            // If roomId is provided, we're just removing the question from this room
            if (roomId.HasValue)
            {
                bool success = await _questionService.RemoveQuestionFromRoomAsync(roomId.Value, id);
                if (!success)
                {
                    TempData["Error"] = "Failed to remove question from room.";
                }
                return RedirectToAction(nameof(Index), new { roomId });
            }
            else
            {
                // If no roomId, we're deleting the question entirely
                bool success = await _questionService.DeleteQuestionAsync(id);
                if (!success)
                {
                    TempData["Error"] = "Failed to delete question. It may be used in active rooms.";
                }
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid? roomId)
        {
            if (roomId.HasValue)
            {
                bool success = await _questionService.RemoveQuestionFromRoomAsync(roomId.Value, id);
                if (!success)
                {
                    TempData["Error"] = "Failed to remove question from room.";
                }
                return RedirectToAction(nameof(Index), new { roomId });
            }
            else
            {
                bool success = await _questionService.DeleteQuestionAsync(id);
                if (!success)
                {
                    TempData["Error"] = "Failed to delete question. It may be used in active rooms.";
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
