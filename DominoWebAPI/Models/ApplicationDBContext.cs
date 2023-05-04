using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DominoWebAPI.Models
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Domino> Domino { get; set; }
        public virtual DbSet<UserInfo> Userinfo { get; set; }
    }
}
