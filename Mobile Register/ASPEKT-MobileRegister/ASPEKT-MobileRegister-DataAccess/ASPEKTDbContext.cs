using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASPEKT_MobileRegister_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPEKT_MobileRegister_DataAccess
{
    public class ASPEKTDbContext : DbContext
    {
        public ASPEKTDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<UserTest> UserTestMobile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserTest>()
                .Property(x => x.FirstName)
                .HasMaxLength(250)
                .IsRequired();
            modelBuilder.Entity<UserTest>()
                .Property(x => x.LastName)
                .HasMaxLength(250)
                .IsRequired();
            modelBuilder.Entity<UserTest>()
                .Property(x => x.Email)
                .HasMaxLength(250)
                .IsRequired();
            modelBuilder.Entity<UserTest>()
                .Property(x => x.Password)
                .HasMaxLength(250)
                .IsRequired();
            modelBuilder.Entity<UserTest>()
                .Property(x => x.PhoneNumber)
                .HasMaxLength(250);

        }
    }
}
