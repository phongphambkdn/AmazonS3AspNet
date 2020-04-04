using Microsoft.EntityFrameworkCore;

namespace AmazonS3AspNet.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<File> FileEntries { get; set; }
    }
}
