using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
 
namespace BattlePlanner.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> UserTable { get; set; }
        public DbSet<Fight> FightTable { get; set; }
        public DbSet<Taunt> TauntTable { get; set; }
        public DbSet<Team> TeamTable { get; set; }
    }
}