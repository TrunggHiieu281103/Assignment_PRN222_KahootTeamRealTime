using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Infrastructures;
using Repositories.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Services.HubSignalR;


namespace Services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IHubContext<QuizHub> _hubContext;

        public QuestionService(UnitOfWork unitOfWork, IHubContext<QuizHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<Question> CreateQuestionAsync(string content, Guid? roomId = null)
        {
            var question = new Question
            {
                Content = content
            };

            var createdQuestion = _unitOfWork.QuestionRepository.AddEntity(question);

            // If roomId is provided, add the question to the room
            if (roomId.HasValue)
            {
                _unitOfWork.QuestionRepository.AddQuestionToRoom(roomId.Value, createdQuestion.Id);
            }

            await _unitOfWork.CompleteAsync();
            return createdQuestion;
        }

        public async Task<Question> GetQuestionByIdAsync(Guid id)
        {
            return _unitOfWork.QuestionRepository.GetQuestionWithAnswers(id);
        }

        public async Task<IEnumerable<Question>> GetQuestionsWithAnswersAsync()
        {
            return _unitOfWork.QuestionRepository.GetQuestionsWithAnswers();
        }

        public async Task<IEnumerable<Question>> GetQuestionsForRoomAsync(Guid roomId)
        {
            return _unitOfWork.QuestionRepository.GetQuestionsForRoom(roomId);
        }

        public async Task AddQuestionToRoomAsync(Guid roomId, Guid questionId)
        {
            _unitOfWork.QuestionRepository.AddQuestionToRoom(roomId, questionId);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<Question>> GetQuestionsByRoomCodeAsync(int roomCode)
        {
            return _unitOfWork.QuestionRepository.GetQuestionsByRoomCode(roomCode);
        }

        public async Task<bool> DeleteQuestionAsync(Guid questionId)
        {
            // Get the question with all relationships
            var question = _unitOfWork.QuestionRepository.GetQuestionWithRelationships(questionId);
            if (question == null)
                return false;

            // Check if the question is used in any active room
            bool isUsedInActiveRoom = question.RoomQuestions
                .Any(rq => rq.Room.IsActive);

            if (isUsedInActiveRoom)
            {
                return false; // Cannot delete questions used in active rooms
            }

            try
            {
                // First remove user answers that reference this question
                _unitOfWork.UserAnswerRepository.RemoveUserAnswersByQuestionId(questionId);

                // Then remove answers
                _unitOfWork.AnswerRepository.RemoveAnswersByQuestionId(questionId);

                // Then remove room-question relationships
                _unitOfWork.QuestionRepository.RemoveRoomQuestions(questionId);

                // Finally remove the question itself
                _unitOfWork.QuestionRepository.Remove(question);

                // Save all changes
                await _unitOfWork.CompleteAsync();

                // Notify clients that a question has been deleted
                await _hubContext.Clients.All.SendAsync("QuestionDeleted", questionId);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveQuestionFromRoomAsync(Guid roomId, Guid questionId)
        {
            var roomQuestions = _unitOfWork.Context.RoomQuestions
                .Where(rq => rq.RoomId == roomId && rq.QuestionId == questionId)
                .ToList();

            if (!roomQuestions.Any())
                return false;

            foreach (var rq in roomQuestions)
            {
                _unitOfWork.Context.RoomQuestions.Remove(rq);
            }

            await _unitOfWork.CompleteAsync();

            // Notify clients
            await _hubContext.Clients.All.SendAsync("QuestionRemovedFromRoom", roomId, questionId);

            return true;
        }
    }
}
