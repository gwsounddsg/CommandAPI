using Microsoft.EntityFrameworkCore;


namespace CommandAPI.Models
{
    public class CommandContext : DbContext
    {
        public DbSet<Command> CommandItems { get; set; }

        
        public CommandContext(DbContextOptions<CommandContext> options) : base(options)
        {
            
        }
    }
}