using Microsoft.EntityFrameworkCore;

namespace Loyola_ERP.Data
{
    public class SchoolManagementContext : DbContext
    {
        public SchoolManagementContext(DbContextOptions<SchoolManagementContext> options)
            : base(options)
        {
        }
    }
}
