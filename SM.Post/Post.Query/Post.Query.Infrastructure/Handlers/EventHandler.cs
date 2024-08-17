
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers
{
  public class EventHandler : IEventHandler
  {
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public EventHandler(IPostRepository postRepository, ICommentRepository commentRepository)
    {
      this.postRepository = postRepository;
      this.commentRepository = commentRepository;
    }

    public async Task On(PostCreatedEvent createdEvent)
    {
      var post = new PostEntity
      {
        PostId = createdEvent.Id,
        Author = createdEvent.Author,
        DatePosted = createdEvent.DatePosted,
        Message = createdEvent.Message
      };

      await postRepository.CreateAsync(post);
    }

    public async Task On(MessageUpdatedEvent messageUpdatedEvent)
    {
      var post = await postRepository.GetByIdAsync(messageUpdatedEvent.Id);
      if (post == null)
      {
        return;
      }

      post.Message = messageUpdatedEvent.Message;
      await postRepository.UpdateAsync(post);
    }

    public async Task On(PostLikedEvent postLikedEvent)
    {
      var post = await postRepository.GetByIdAsync(postLikedEvent.Id);
      if (post == null)
      {
        return;
      }

      post.Like++;
      await postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent commentAddedEvent)
    {
      var comment = new CommentEntity
      {
        PostId = commentAddedEvent.Id,
        CommentId = commentAddedEvent.CommentId,
        CommentDate = commentAddedEvent.CommentDate,
        Comment = commentAddedEvent.Comment,
        Username = commentAddedEvent.Username,
        Edited = false
      };

      await commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentUpdatedEvent commentUpdatedEvent)
    {
      var comment = await commentRepository.GetByIdAsync(commentUpdatedEvent.CommentId);
      if (comment == null)
      {
        return;
      }

      comment.Comment = commentUpdatedEvent.Comment;
      comment.Edited = true;
      comment.CommentDate = commentUpdatedEvent.EditDate;

      await commentRepository.UpdateAsync(comment);
    }

    public async Task On(CommentRemovedEvent commentRemovedEvent)
    {
      await commentRepository.DeleteAsync(commentRemovedEvent.CommentId);
    }

    public async Task On(DeletePostEvent deletePostEvent)
    {
      await postRepository.DeleteAsync(deletePostEvent.Id);
    }
  }
}
