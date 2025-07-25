using Microsoft.EntityFrameworkCore;
using People.Data.Entities;

namespace People.Data.Context
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Person> MyEntities { get; set; }
    }
}
