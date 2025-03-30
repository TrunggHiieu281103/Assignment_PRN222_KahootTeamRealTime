using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class QuestionScoresModel : PageModel
    {
        public List<(string Name, int Points)> UserPoints { get; set; } = new();

        public void OnGet()
        {
            UserPoints.Add(("User1", 10));
            UserPoints.Add(("User2", 5));
        }
    }
}
