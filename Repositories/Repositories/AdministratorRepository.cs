using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Repositories.Infrastructures;
using Repositories.Models;

namespace Repositories.Repositories
{
    public class AdministratorRepository : GenericRepository<Administrator>
    {
        public AdministratorRepository(RealtimeQuizDbContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public Administrator GetByUsername(string username)
        {
            return _context.Administrators
                .Include(a => a.Role)
                .FirstOrDefault(a => a.UserName == username);
        }

        public Administrator Authenticate(string username, string password)
        {
            return _context.Administrators
                .Include(a => a.Role)
                .FirstOrDefault(a => a.UserName == username &&
                                    a.Password == password &&
                a.IsActive);
        }

        public IEnumerable<Administrator> GetAllWithRoles()
        {
            return _context.Administrators
                .Include(a => a.Role)
                .ToList();
        }

        public void UpdateIsActive(int adminId, bool isActive)
        {
            var admin = GetEntityById(adminId);
            if (admin != null)
            {
                admin.IsActive = isActive;
                var entry = _context.Entry(admin);
                entry.Property(a => a.IsActive).IsModified = true;
            }
        }
    }
}
