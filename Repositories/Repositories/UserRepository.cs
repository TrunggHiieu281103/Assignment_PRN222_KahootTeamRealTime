using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Infrastructures;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(RealtimeQuizDbContext context, ILogger logger)
        : base(context, logger)
        {
        }

        public User GetUserByUserName(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }


    }
}
