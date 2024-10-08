﻿using Post.Query.Domain.Entities;

namespace Post.Query.Api.Queries
{
  public interface IQueryHandler
  {
    Task<List<PostEntity>> HandlerAsync(FindAllPostsQuery  query);
    Task<List<PostEntity>> HandlerAsync(FindPostByIdQuery query);
    Task<List<PostEntity>> HandlerAsync(FindPostsByAuthorQuery query);
    Task<List<PostEntity>> HandlerAsync(FindPostsWithCommentsQuery query);
    Task<List<PostEntity>> HandlerAsync(FindPostsWithLikesQuery query);
  }
}
