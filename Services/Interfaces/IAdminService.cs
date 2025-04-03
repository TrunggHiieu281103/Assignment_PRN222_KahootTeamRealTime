using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories.Models;

namespace Services.Interfaces
{
    public interface IAdminService
    {
        Task<Administrator> AuthenticateAsync(string username, string password);
        Task<IEnumerable<Administrator>> GetAllAdministratorsAsync();
        Task<Administrator> GetAdministratorByIdAsync(int id);
        Task<Administrator> CreateAdministratorAsync(Administrator admin);
        Task<bool> UpdateAdministratorAsync(Administrator admin);
        Task<bool> ToggleAdminActiveStatusAsync(int adminId);
    }
}
