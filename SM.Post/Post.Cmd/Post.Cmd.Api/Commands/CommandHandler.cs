using CQRS.Core.Handlers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands
{
  public class CommandHandler : ICommandHandler
  {
    private readonly IEventSourcingHandler<PostAggregate> eventSourcingHandler;

    public CommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
      this.eventSourcingHandler = eventSourcingHandler;
    }

    public async Task HandleAsync(AddCommentCommand command)
    {
      var aggregate = await this.eventSourcingHandler.GetByIdAsync(command.Id);
      aggregate.AddComment(command.Comment, command.Username);
      
      await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(DeletePostCommand command)
    {
      var aggregate = await this.eventSourcingHandler.GetByIdAsync(command.Id);
      aggregate.DeletePost(command.Username);

      await this.eventSourcingHandler.SaveAsync(aggregate);
    }
    public async Task HandleAsync(EditMessageCommand command)
    {
      var aggregate = await this.eventSourcingHandler.GetByIdAsync(command.Id);
      aggregate.EditMessage(command.Message);

      await this.eventSourcingHandler.SaveAsync(aggregate);
    }
    public async Task HandleAsync(LikePostCommand command)
    {
      var aggregate = await this.eventSourcingHandler.GetByIdAsync(command.Id);
      aggregate.LikePost();

      await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(RemoveCommentCommand command)
    {
      var aggregate = await this.eventSourcingHandler.GetByIdAsync(command.Id);
      aggregate.RemoveComment(command.Id,command.Username);

      await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(NewPostCommand command)
    {
      var aggregate = new PostAggregate(command.Id, command.Author, command.Message);
      
      await this.eventSourcingHandler.SaveAsync(aggregate);
    }

    public async Task HandleAsync(EditCommentCommand command)
    {
      var aggregate = await this.eventSourcingHandler.GetByIdAsync(command.Id);
      aggregate.EditComment(command.Id, command.Comment, command.UserName);

      await this.eventSourcingHandler.SaveAsync(aggregate);
    }
  }
}
