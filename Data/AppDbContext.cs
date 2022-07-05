using System;
using jwt_auth.Models;
using Microsoft.EntityFrameworkCore;

namespace jwt_auth.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
            
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<User>? Users { get; set; }
    }
}
