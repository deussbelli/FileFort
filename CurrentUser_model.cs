using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrentUser_model
{
    public class CurrentUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime? BlockedUntil { get; set; }
        public bool IsAdmin { get; set; }

        public byte[] Sid { get; set; }
    }

    public class UserDbContext : DbContext
    {
        public DbSet<CurrentUser> Users { get; set; }
    }
}
