using CQRS.Core.Events;

namespace Post.Common.Events
{
  public class DeletePostEvent : BaseEvent
  {
    public DeletePostEvent() : base(nameof(DeletePostEvent))
    {
    }
  }
}
