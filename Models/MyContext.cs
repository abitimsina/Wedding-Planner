using Microsoft.EntityFrameworkCore;
 
namespace WeddingPlanner.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> users {get;set;}
        public DbSet<Wedding> wedding {get;set;}
        public DbSet<Reservation> reservation {get;set;}
    }
}
