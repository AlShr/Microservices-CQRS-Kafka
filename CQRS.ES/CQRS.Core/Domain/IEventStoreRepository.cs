using CQRS.Core.Events;

namespace CQRS.Core.Domain
{
  public interface IEventStoreRepository
  {
    Task SaveAsync(EventModel model);
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
  }
}
