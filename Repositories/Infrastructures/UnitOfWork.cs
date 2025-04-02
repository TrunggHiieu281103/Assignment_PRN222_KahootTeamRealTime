using Microsoft.Extensions.Logging;
using Repositories.Models;
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


        public UnitOfWork(RealtimeQuizDbContext context, ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("log");
        }


        public async Task CompleteAsync() => await _context.SaveChangesAsync();
    }
}
