using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Services.HubSignalR
{
    public class QuizHub : Hub
    {
        public async Task QuestionDeleted(Guid questionId)
        {
            await Clients.All.SendAsync("ReceiveQuestionDeleted", questionId);
        }

        public async Task QuestionRemovedFromRoom(Guid roomId, Guid questionId)
        {
            await Clients.All.SendAsync("ReceiveQuestionRemovedFromRoom", roomId, questionId);
        }
        public async Task AnswerDeleted(Guid answerId, Guid questionId)
        {
            await Clients.All.SendAsync("ReceiveAnswerDeleted", answerId, questionId);
        }
    }
}
