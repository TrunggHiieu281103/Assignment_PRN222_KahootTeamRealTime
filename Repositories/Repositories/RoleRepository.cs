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
    public class RoleRepository : GenericRepository<Role>
    {
        public RoleRepository(RealtimeQuizDbContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public IEnumerable<Role> GetActiveRoles()
        {
            return _dbSet.Where(r => r.IsActive).ToList();
        }

        public Role GetRoleByName(string roleName)
        {
            return _dbSet.FirstOrDefault(r => r.RoleName == roleName);
        }
    }
}
