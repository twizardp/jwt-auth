using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.DbContext
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connStr = "server=192.168.150.150;port=3306;uid=root;pwd=sa@123456;database=jwt-auth";
            optBuilder.UseMySql(connStr, MySqlServerVersion.AutoDetect(connStr));

            return new ApplicationDbContext(optBuilder.Options);
        }
    }
}
