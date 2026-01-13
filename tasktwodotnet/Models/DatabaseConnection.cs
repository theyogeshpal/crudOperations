using Microsoft.EntityFrameworkCore;

namespace tasktwodotnet.Models
{
    public class DatabaseConnection : DbContext
    {
        public DatabaseConnection(DbContextOptions<DatabaseConnection> options) : base(options)
        {
        }
        public DbSet<User> User { get; set; }
    }
}
