using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Infrastructures;
using Repositories.Models;
using Services.Interfaces;

namespace Services.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly UnitOfWork _unitOfWork;

        public QuestionService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
