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
            var admin = _unitOfWork.AdministratorRepository.GetEntityById(adminId);
            if (admin == null)
                return false;

            _unitOfWork.AdministratorRepository.UpdateIsActive(adminId, !admin.IsActive);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
