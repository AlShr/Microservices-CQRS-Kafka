
using Post.Common.Events;

namespace Post.Query.Infrastructure.Handlers
{
  public interface IEventHandler
  {
    Task On(PostCreatedEvent createdEvent);

    Task On(MessageUpdatedEvent messageUpdatedEvent);

    Task On(PostLikedEvent postLikedEvent);

    Task On(CommentAddedEvent commentAddedEvent);

    Task On(CommentUpdatedEvent commentUpdatedEvent);

    Task On(CommentRemovedEvent commentRemovedEvent);

    Task On(DeletePostEvent deletePostEvent);
  }
}
