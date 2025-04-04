using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Repositories.Infrastructures;
using Repositories.Models;
using Services.HubSignalR;
using Services.Interfaces;

namespace Services.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IHubContext<QuizHub> _hubContext;

        public AnswerService(UnitOfWork unitOfWork, IHubContext<QuizHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<Answer> CreateAnswerAsync(Guid questionId, string content, bool isCorrect)
        {
            var answer = new Answer
            {
                QuestionId = questionId,
                Content = content,
                IsCorrect = isCorrect
            };

            var createdAnswer = _unitOfWork.AnswerRepository.AddEntity(answer);
            await _unitOfWork.CompleteAsync();
            return createdAnswer;
        }

        public async Task<Answer> GetAnswerByIdAsync(Guid id)
        {
            var answer = _unitOfWork.AnswerRepository.GetAnswerWithRelationships(id);

            return answer;
        }

        public async Task<IEnumerable<Answer>> GetAnswersForQuestionAsync(Guid questionId)
        {
            return _unitOfWork.AnswerRepository.GetAnswersForQuestion(questionId);
        }

        public async Task<bool> DeleteAnswerAsync(Guid answerId)
        {
            var answer = _unitOfWork.AnswerRepository.GetAnswerWithRelationships(answerId);
            if (answer == null)
                return false;

            // Check if answer is used in an active room through its question
            bool isUsedInActiveRoom = _unitOfWork.Context.RoomQuestions
                .Where(rq => rq.QuestionId == answer.QuestionId && rq.Room.IsActive)
                .Any();

            if (isUsedInActiveRoom)
                return false; // Cannot delete answers used in active rooms

            try
            {
                // Get the questionId before removing the answer (for notifications)
                var questionId = answer.QuestionId;

                // Handle UserAnswers that reference this answer
                _unitOfWork.AnswerRepository.RemoveUserAnswersByAnswerId(answerId);

                // Remove the answer
                _unitOfWork.AnswerRepository.Remove(answer);

                // Save changes
                await _unitOfWork.CompleteAsync();

                // Notify clients that an answer has been deleted
                await _hubContext.Clients.All.SendAsync("AnswerDeleted", answerId, questionId);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
