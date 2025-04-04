using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByUserName(string username);
        Task<User> CreateUserAsync(string username);

    }
}
