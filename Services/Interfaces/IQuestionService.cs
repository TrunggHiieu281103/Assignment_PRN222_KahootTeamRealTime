using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Models;

namespace Services.Interfaces
{
    public interface IQuestionService
    {
        Task<Question> CreateQuestionAsync(string content, Guid? roomId = null);
        Task<Question> GetQuestionByIdAsync(Guid id);
        Task<IEnumerable<Question>> GetQuestionsWithAnswersAsync();
        Task<IEnumerable<Question>> GetQuestionsForRoomAsync(Guid roomId);
        Task AddQuestionToRoomAsync(Guid roomId, Guid questionId);
        Task<IEnumerable<Question>> GetQuestionsByRoomCodeAsync(int roomCode);

    }
}
