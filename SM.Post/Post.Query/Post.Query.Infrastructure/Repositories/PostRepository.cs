using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAcceess;

namespace Post.Query.Infrastructure.Repositories
{
  public class PostRepository : IPostRepository
  {
    private readonly DatabaseContextFactory contextFactory;

    public PostRepository(DatabaseContextFactory contextFactory) 
    {
      this.contextFactory = contextFactory;
    }

    public async Task CreateAsync(PostEntity postEntity)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      context.Posts.Add(postEntity);
      _=await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      var post = await GetByIdAsync(postId);

      if (post == null) return;

      context.Posts.Remove(post);
      _ = await context.SaveChangesAsync();
    }

    public async Task<PostEntity> GetByIdAsync(Guid postId)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      return await context.Posts
        .Include(p => p.Comments)
        .FirstOrDefaultAsync(x => x.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      return await context.Posts
        .AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      return await context.Posts
        .AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .Where(x=>x.Author.Contains(author))
        .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      return await context.Posts
        .AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .Where(x => x.Comments != null && x.Comments.Any())
        .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      return await context.Posts
        .AsNoTracking()
        .Include(p => p.Comments)
        .AsNoTracking()
        .Where(x => x.Like >= numberOfLikes)
        .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity postEntity)
    {
      using DatabaseContext context = this.contextFactory.CreateDbContext();
      context.Posts.Update(postEntity);

      await context.SaveChangesAsync();
    }
  }
}
