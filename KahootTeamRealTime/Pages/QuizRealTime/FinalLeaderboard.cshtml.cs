using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KahootTeamRealTime.Pages.QuizRealTime
{
    public class FinalLeaderboardModel : PageModel
    {
        public List<(string Name, int Points)> Top3 { get; set; } = new();
        public List<(string Name, int Points)> AllPlayers { get; set; } = new();

        public void OnGet()
        {
            AllPlayers = new List<(string Name, int Points)>
            {
                ("User1", 50),
                ("User2", 40),
                ("User3", 30),
                ("User4", 20),
                ("User5", 10)
            };

            Top3 = AllPlayers.OrderByDescending(u => u.Points).Take(3).ToList();
        }
    }
}
