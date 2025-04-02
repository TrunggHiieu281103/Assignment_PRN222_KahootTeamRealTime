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
    public class AnswerService : IAnswerService
    {
        private readonly UnitOfWork _unitOfWork;

        public AnswerService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public async Task<IEnumerable<Answer>> GetAnswersForQuestionAsync(Guid questionId)
        {
            return _unitOfWork.AnswerRepository.GetAnswersForQuestion(questionId);
        }
    }
}
