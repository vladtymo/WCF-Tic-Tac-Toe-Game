namespace DAL
{
    using System.Data.Entity;
    using System.Linq;

    // контекст бази даних
    public class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        // колекція ігор в базі даних
        public virtual DbSet<GameInfo> Games { get; set; }
    }
}