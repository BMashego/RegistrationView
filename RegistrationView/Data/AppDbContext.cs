using Microsoft.EntityFrameworkCore;
using RegistrationView.Models;

namespace RegistrationView.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        //public DbSet<CustomerModel> Customers { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> contextOptions) 
            : base(contextOptions) { }
    }
}
