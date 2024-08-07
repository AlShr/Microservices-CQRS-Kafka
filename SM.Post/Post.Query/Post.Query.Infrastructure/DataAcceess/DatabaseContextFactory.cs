
using Microsoft.EntityFrameworkCore;

namespace Post.Query.Infrastructure.DataAcceess
{
  public class DatabaseContextFactory
  {
    private readonly Action<DbContextOptionsBuilder> configureDbContext;

    public DatabaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext) 
    {
      this.configureDbContext = configureDbContext;
    }

    public DatabaseContext CreateDbContext()
    {
      DbContextOptionsBuilder<DatabaseContext> optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
      this.configureDbContext(optionsBuilder);

      return new DatabaseContext(optionsBuilder.Options);
    }
  }
}
