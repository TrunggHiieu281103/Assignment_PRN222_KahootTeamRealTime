using Microsoft.EntityFrameworkCore;
using Repositories.Infrastructures;
using Repositories.Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService : IUserService
    {
        private readonly UnitOfWork _unitOfWork;

        public UserService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<User> GetUserByUserName(string username)
        {
            return _unitOfWork.UserRepository.GetUserByUserName(username);
        }

        public async Task<User> CreateUserAsync(string username)
        {
            var user = new User { Username = username };

            var newUser = _unitOfWork.UserRepository.AddEntity(user);
            await _unitOfWork.CompleteAsync();
            return newUser;
        }
    }
}
