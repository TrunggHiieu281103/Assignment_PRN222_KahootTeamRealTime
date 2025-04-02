using Microsoft.AspNetCore.SignalR;
using Repositories.Models;

namespace KahootTeamRealTime.HubSginalR
{
    public class QuizHub:Hub
    {
        public async Task JoinRoom(int roomCode)
        {
            await Clients.All.SendAsync("ReceiveUserJoined", roomCode);
        }

    }
}
