using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates
{
  public class PostAggregate : AggregateRoot
  {
    private readonly Dictionary<Guid, Tuple<string, string>> comments = new();

    private bool active;

    private string? author;

    public bool Active
    {
      get => active; set => active = value;
    }

    public PostAggregate() { }

    public PostAggregate(Guid id, string author, string message)
    {
      this.RaiseEvent(new PostCreatedEvent
      {
        Id = id,
        Author = author,
        Message = message,
        DatePosted = DateTime.Now
      });
    }

    public void Apply(PostCreatedEvent postCreatedEvent)
    {
      this.Id = postCreatedEvent.Id;
      this.author = postCreatedEvent.Author;
      this.active = true;
    }

    public void Apply(MessageUpdatedEvent messageUpdatedEvent)
    {
      this.Id = messageUpdatedEvent.Id;
    }

    public void Apply(PostLikedEvent postLikedEvent)
    {
      this.Id = postLikedEvent.Id;
    }

    public void Apply(CommentAddedEvent commentAddedEvent)
    {
      this.Id = commentAddedEvent.Id;
      comments.Add(commentAddedEvent.CommentId, new Tuple<string, string>(commentAddedEvent.Comment, commentAddedEvent.Username));
    }

    public void Apply(CommentUpdatedEvent commentUpdatedEvent)
    {
      this.Id = commentUpdatedEvent.Id;
      comments[commentUpdatedEvent.CommentId] = new Tuple<string, string>(commentUpdatedEvent.Comment, commentUpdatedEvent.UserName);
    }

    public void Apply(CommentRemovedEvent commentRemovedEvent) 
    {
      this.Id = commentRemovedEvent.Id;
      comments.Remove(commentRemovedEvent.CommentId);
    }

    public void Apply(DeletePostEvent deletePostEvent) 
    {
      this.Id = deletePostEvent.Id;
      this.active = false;
    }

    public void AddComment(string comment, string username) 
    {
      if (this.active == false) 
      {
        throw new InvalidOperationException("You cannot add comment to an inactive post");
      }
      if (string.IsNullOrEmpty(comment))
      {
        throw new InvalidOperationException($"The value of {nameof(comment)} cannot be null or empty");
      }

      this.RaiseEvent(new CommentAddedEvent 
      { 
        Id = this.Id,
        CommentId = Guid.NewGuid(),
        Comment = comment,
        Username = username,
        CommentDate = DateTime.Now
      });
    }

    public void EditComment(Guid commentId, string comment, string userName) 
    {
      if (this.active == false)
      {
        throw new InvalidOperationException("You cannot add comment to an inactive post");
      }
      if (string.IsNullOrEmpty(comment))
      {
        throw new InvalidOperationException($"The value of {comment} cannot be null or empty");
      }
      if (comments[commentId].Item2.Equals(userName,StringComparison.CurrentCultureIgnoreCase) == false) 
      {
        throw new InvalidOperationException("You are not allowed to edit a comment that was made by another user");
      }


      this.RaiseEvent(new CommentUpdatedEvent
      {
        Id = this.Id,
        CommentId = commentId,
        Comment = comment,
        UserName = userName,
        EditDate = DateTime.Now
      });
    }

    public void EditMessage(string message) 
    {
      if(this.active == false) 
      {
        throw new InvalidOperationException("You cannot edit the message of an inactive post");
      }
      if(string.IsNullOrEmpty(message)) 
      {
        throw new InvalidOperationException($"The value of {nameof(message)} cannot be null or empty");
      }

      this.RaiseEvent(new MessageUpdatedEvent 
      { 
        Id = this.Id, 
        Message = message 
      });
    }

    public void LikePost()
    {
      if(this.active == false) 
      {
        throw new InvalidOperationException("You cannot like an inactive post");
      }

      RaiseEvent(new PostLikedEvent
      {
        Id = this.Id,
      });
    }

    public void RemoveComment(Guid commentId, string username) 
    {
      if (this.active == false)
      {
        throw new InvalidOperationException("You cannot remove the comment of an inactive post");
      }

      if (comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase) == false)
      {
        throw new InvalidOperationException("You are not allowed to remove a comment that was made by another user");
      }

      RaiseEvent(new CommentRemovedEvent
      {
        Id = this.Id,
        CommentId = commentId
      });
    }

    public void DeletePost(string userName) 
    {
      if (this.active == false)
      {
        throw new InvalidOperationException("The post has already been removed");
      }
      if (this.author.Equals(userName, StringComparison.CurrentCultureIgnoreCase) == false)
      {
        throw new InvalidOperationException("You are not allowed to delete a post that was made by another user");
      }

      RaiseEvent(new DeletePostEvent 
      {
        Id = this.Id
      });
    }
  }
}
