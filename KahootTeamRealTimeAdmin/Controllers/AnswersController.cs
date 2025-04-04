using KahootTeamRealTimeAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace KahootTeamRealTimeAdmin.Controllers
{
    public class AnswersController : Controller
    {
        private readonly IAnswerService _answerService;
        private readonly IQuestionService _questionService;

        public AnswersController(IAnswerService answerService, IQuestionService questionService)
        {
            _answerService = answerService;
            _questionService = questionService;
        }

        // GET: Answers/Create
        public async Task<IActionResult> Create(Guid questionId)
        {
            var question = await _questionService.GetQuestionByIdAsync(questionId);
            if (question == null)
            {
                return NotFound();
            }

            ViewBag.Question = question;
            return View(new AnswerViewModel { QuestionId = questionId });
        }

        // POST: Answers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AnswerViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _answerService.CreateAnswerAsync(model.QuestionId, model.Content, model.IsCorrect);
                return RedirectToAction("Details", "Questions", new { id = model.QuestionId });
            }

            var question = await _questionService.GetQuestionByIdAsync(model.QuestionId);
            ViewBag.Question = question;
            return View(model);
        }// GET: Answers/Delete/{id}
        public async Task<IActionResult> Delete(Guid id, Guid? roomId)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
                return NotFound();

            var questionId = answer.QuestionId;

            bool success = await _answerService.DeleteAnswerAsync(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete answer. It may be used in active rooms.";
            }

            // Redirect back to the question details
            return RedirectToAction("Details", "Questions", new { id = questionId, roomId });
        }

        // POST: Answers/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, Guid? roomId)
        {
            var answer = await _answerService.GetAnswerByIdAsync(id);
            if (answer == null)
                return NotFound();

            var questionId = answer.QuestionId;

            bool success = await _answerService.DeleteAnswerAsync(id);
            if (!success)
            {
                TempData["Error"] = "Failed to delete answer. It may be used in active rooms.";
            }

            // Redirect back to the question details
            return RedirectToAction("Details", "Questions", new { id = questionId, roomId });
        }


    }
}
