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
    public class RoleService : IRoleService
    {
        private readonly UnitOfWork _unitOfWork;

        public RoleService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return _unitOfWork.RoleRepository.GetAll();
        }

        public async Task<Role> GetRoleByIdAsync(int id)
        {
            return _unitOfWork.RoleRepository.GetEntityById(id);
        }

        public async Task<Role> GetRoleByNameAsync(string roleName)
        {
            return _unitOfWork.RoleRepository.GetRoleByName(roleName);
        }

        public async Task<IEnumerable<Role>> GetActiveRolesAsync()
        {
            return _unitOfWork.RoleRepository.GetActiveRoles();
        }
    }
}
