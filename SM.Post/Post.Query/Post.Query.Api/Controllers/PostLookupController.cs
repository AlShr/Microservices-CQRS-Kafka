using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Common.DTOs;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class PostLookupController : ControllerBase
  {
    private readonly ILogger<PostLookupController> logger;
    private readonly IQueryDispatcher<PostEntity> queryDispatcher;

    public PostLookupController(ILogger<PostLookupController> logger, IQueryDispatcher<PostEntity> queryDispatcher)
    {
      this.logger = logger;
      this.queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    public async Task<ActionResult> GetAllPostAsync()
    {
      try
      {
        var posts = await queryDispatcher.SendAsync(new FindAllPostsQuery());

        return this.NormalResponse(posts);
      }
      catch (Exception ex)
      {
        const string safeErrorMessage = "Error while processing request to retrieve all posts";
        return this.ErrorResponse(ex, safeErrorMessage);
      }
    }

    [HttpGet("byId/{postId}")]
    public async Task<ActionResult> GetByPostIdAsync(Guid postId)
    {
      try
      {
        var posts = await queryDispatcher.SendAsync(new FindPostByIdQuery{Id = postId});

        return this.NormalResponse(posts);
      }
      catch (Exception ex)
      {
        const string safeErrorMessage = "Error while processing request to find post by Id";
        return this.ErrorResponse(ex, safeErrorMessage);
      }
    }

    [HttpGet("byAuthor/{author}")]
    public async Task<ActionResult> GetPostsByAuthorAsync(string author)
    {
      try
      {
        var posts = await queryDispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author });

        return this.NormalResponse(posts);
      }
      catch (Exception ex)
      {
        const string safeErrorMessage = "Error while processing request to find posts by author";
        return this.ErrorResponse(ex, safeErrorMessage);
      }
    }

    [HttpGet("withComments")]
    public async Task<ActionResult> GetPostsWithCommentsAsync()
    {
      try
      {
        var posts = await queryDispatcher.SendAsync(new FindPostsWithCommentsQuery());
        return this.NormalResponse(posts);
      }
      catch (Exception ex)
      {
        const string safeErrorMessage = "Error while processing request to find posts with comments";
        return this.ErrorResponse(ex, safeErrorMessage);
      }
    }

    [HttpGet("withLikes/{numberOfLikes}")]
    public async Task<ActionResult> GetPostsWithLikesAsync(int numberOfLikes)
    {
      try
      {
        var posts = await queryDispatcher.SendAsync(new FindPostsWithLikesQuery { NumberOfLikes = numberOfLikes });
        return this.NormalResponse(posts);
      }
      catch (Exception ex)
      {
        const string safeErrorMessage = "Error while processing request to find posts with likes";
        return this.ErrorResponse(ex, safeErrorMessage);
      }
    }

    private ActionResult NormalResponse(List<PostEntity> posts)
    {
      if (posts == null || posts.Any() == false)
      {
        return NoContent();
      }

      var count = posts.Count;

      return Ok(new PostLookupResponse()
      {
        Posts = posts,
        Message = $"Successfully returned {count} post{(count > 1 ? "s" : string.Empty)}"
      });
    }

    private ActionResult ErrorResponse(Exception ex, string safeErrorMessage)
    {
      logger.Log(LogLevel.Error, safeErrorMessage, ex);
      return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
      {
        Message = safeErrorMessage
      });
    }
  }
}
