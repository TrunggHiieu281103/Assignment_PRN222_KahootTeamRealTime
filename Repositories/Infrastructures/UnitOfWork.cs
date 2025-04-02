using Microsoft.Extensions.Logging;
using Repositories.Models;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Infrastructures
{
    public class UnitOfWork
    {
        private readonly RealtimeQuizDbContext _context;
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;

        private RoomRepository _roomRepository;
        private QuestionRepository _questionRepository;
        private AnswerRepository _answerRepository;



        public UnitOfWork(RealtimeQuizDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("log");
            _loggerFactory = loggerFactory;
        }

        public RoomRepository RoomRepository
        {
            get
            {
                if (_roomRepository == null)
                {
                    _roomRepository = new RoomRepository(_context, _logger);
                }
                return _roomRepository;
            }
        }

        public QuestionRepository QuestionRepository
        {
            get
            {
                if (_questionRepository == null)
                {
                    _questionRepository = new QuestionRepository(_context, _logger);
                }
                return _questionRepository;
            }
        }

        public AnswerRepository AnswerRepository
        {
            get
            {
                if (_answerRepository == null)
                {
                    _answerRepository = new AnswerRepository(_context, _logger);
                }
                return _answerRepository;
            }
        }
        public async Task CompleteAsync() => await _context.SaveChangesAsync();
    }
}
