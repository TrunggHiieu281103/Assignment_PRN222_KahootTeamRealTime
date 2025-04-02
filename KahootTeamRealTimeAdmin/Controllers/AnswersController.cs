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
        }
    }
}
