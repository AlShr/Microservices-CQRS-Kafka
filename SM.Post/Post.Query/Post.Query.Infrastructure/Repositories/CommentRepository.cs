
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAcceess;

namespace Post.Query.Infrastructure.Repositories
{
  public class CommentRepository : ICommentRepository
  {
    private readonly DatabaseContextFactory contextFactory;

    public CommentRepository(DatabaseContextFactory contextFactory) 
    {
      this.contextFactory = contextFactory;
    }

    public async Task CreateAsync(CommentEntity commentEntity)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      context.Comments.Add(commentEntity);

      _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid commentId)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      var comment = await GetByIdAsync(commentId);
      if (comment == null) return;
      context.Comments.Remove(comment);

      _= await context.SaveChangesAsync();
    }

    public async Task<CommentEntity> GetByIdAsync(Guid commentId)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      return await context.Comments.FirstOrDefaultAsync(x => x.CommentId == commentId);
    }

    public async Task UpdateAsync(CommentEntity commentEntity)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      context.Comments.Update(commentEntity);

      _= await context.SaveChangesAsync();
    }
  }
}
