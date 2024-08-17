using Microsoft.AspNetCore.Mvc;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Api.Queries
{
  public class QueryHandler : IQueryHandler
  {
    private readonly IPostRepository postRepository;

    public QueryHandler(IPostRepository postRepository)
    {
      this.postRepository = postRepository;
    }

    public async Task<List<PostEntity>> HandlerAsync(FindAllPostsQuery query)
    {
      return await postRepository.ListAllAsync();
    }

    public async Task<List<PostEntity>> HandlerAsync(FindPostByIdQuery query)
    {
      var post = await postRepository.GetByIdAsync(query.Id);
      return new List<PostEntity> { post };
    }

    public async Task<List<PostEntity>> HandlerAsync(FindPostsByAuthorQuery query)
    {
      return await postRepository.ListByAuthorAsync(query.Author);
    }

    public async Task<List<PostEntity>> HandlerAsync(FindPostsWithCommentsQuery query)
    {
      return await postRepository.ListWithCommentsAsync();
    }

    public async Task<List<PostEntity>> HandlerAsync(FindPostsWithLikesQuery query)
    {
      return await postRepository.ListWithLikesAsync(query.NumberOfLikes);
    }
  }
}
