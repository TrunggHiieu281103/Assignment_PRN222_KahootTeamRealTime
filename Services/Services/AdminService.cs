using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Infrastructures;
using Repositories.Models;
using Services.Interfaces;

namespace Services.Services
{
    public class AdminService : IAdminService
    {
        private readonly UnitOfWork _unitOfWork;

        public AdminService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Administrator> AuthenticateAsync(string username, string password)
        {
            return _unitOfWork.AdministratorRepository.Authenticate(username, password);
        }

        public async Task<IEnumerable<Administrator>> GetAllAdministratorsAsync()
        {
            return _unitOfWork.AdministratorRepository.GetAllWithRoles();
        }

        public async Task<Administrator> GetAdministratorByIdAsync(int id)
        {
            return _unitOfWork.AdministratorRepository.GetEntityById(id);
        }

        public async Task<Administrator> CreateAdministratorAsync(Administrator admin)
        {
            var createdAdmin = _unitOfWork.AdministratorRepository.AddEntity(admin);
            await _unitOfWork.CompleteAsync();
            return createdAdmin;
        }

        public async Task<bool> UpdateAdministratorAsync(Administrator admin)
        {
            _unitOfWork.AdministratorRepository.UpdateEntity(admin);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> ToggleAdminActiveStatusAsync(int adminId)
        {
            //var admin = _unitOfWork.AdministratorRepository.GetEntityById(adminId);
            //if (admin == null)
            //    return false;

            //_unitOfWork.AdministratorRepository.UpdateIsActive(adminId, !admin.IsActive);
            //await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<Administrator> RegisterAsync(Administrator administrator)
        {
            if (administrator == null)
                throw new ArgumentNullException(nameof(administrator));

    
            var existingAdmin = await _unitOfWork.AdministratorRepository.GetByUsernameAsync(administrator.UserName);

            if (existingAdmin != null)
                throw new InvalidOperationException("Username already exists.");

            
            //administrator.Password = HashPassword(administrator.Password);

            await _unitOfWork.AdministratorRepository.CreateAsync(administrator);
            await _unitOfWork.CompleteAsync();

            return administrator;
        }

        //private string HashPassword(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        //    }
        //}

    }
}
