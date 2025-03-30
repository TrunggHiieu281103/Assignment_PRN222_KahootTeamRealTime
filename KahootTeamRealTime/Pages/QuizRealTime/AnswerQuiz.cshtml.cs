using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class AnswerQuizModel : PageModel
    {
        public string Question { get; set; }
        public List<(string Id, string Content)> Answers { get; set; } = new();

        [BindProperty]
        public string SelectedAnswer { get; set; }

        public void OnGet(string roomId)
        {
            Question = "What is 2 + 2?";
            Answers.Add(("1", "3"));
            Answers.Add(("2", "4"));
            Answers.Add(("3", "5"));
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("QuestionScores");
        }
    }
}
