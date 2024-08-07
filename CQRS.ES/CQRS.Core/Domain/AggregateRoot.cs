using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
  public abstract class AggregateRoot
  {
    private readonly List<BaseEvent> changes = new ();

    private Guid id;
    public Guid Id { get { return id; } set { this.id = value; } }

    public int Version { get; set; } = -1;

    public void MarkChangesAsCommitted() => changes.Clear();

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
      foreach (BaseEvent e in events)
      {
        this.ApplyChange(e, false);
      }
    }

    public IEnumerable<BaseEvent> GetUncommittedChanges() 
    {
      return changes;
    }


    protected void RaiseEvent(BaseEvent e)
    {
      this.ApplyChange(e, true);
    }

    private void ApplyChange(BaseEvent @event, bool isNew)
    {
      var method = this.GetType().GetMethod("Apply", new Type[] { @event.GetType() });
      if (method == null) 
      { 
        throw new ArgumentNullException(nameof(@event));
      }
      method.Invoke(this, new object[] { @event });

      if(isNew) 
      {
        changes.Add(@event);
      }
    }
  }
}
