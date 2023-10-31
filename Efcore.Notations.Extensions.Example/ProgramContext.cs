using Efcore.Notation.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Efcore.Notations.Extensions.Example
{
    /// <summary>
    /// Program context.
    /// </summary>
    public class ProgramContext : ExtendedDbContext
    {
        /// <summary>
        /// Initialize a new <see cref="ProgramContext"/> instance.
        /// </summary>
        public ProgramContext()
        {
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder Models, ExtendedDbEntitySet Types)
        {
            // --> add all entities which have `Table` notation.
            Types.AddFrom(typeof(Program).Assembly);
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder Options)
        {
            base.OnConfiguring(Options);
            /*var ConnStr = (new MySqlConnectionStringBuilder()
            {
                Server = "127.0.0.1",
                Port = 3306,
                UserID = "test",
                Password = "test1!",
                Database = "test",
                CharacterSet = "utf8mb4"
            }).ToString();

            Options.UseMySql(ConnStr, ServerVersion.AutoDetect(ConnStr));*/

            Options.UseSqlite("Data Source=Program.db");
        }
    }
}