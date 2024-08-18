using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers
{
  public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
  {
    private readonly IEventStore eventStore;
    private readonly IEventProducer eventProducer;

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
      this.eventStore = eventStore;
      this.eventProducer = eventProducer;
    }

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
      var aggregate = new PostAggregate();
      var events = await this.eventStore.GetEventsAsync(aggregateId);

      if (events == null || events.Any() == false) 
      {
        return aggregate;
      }

      aggregate.ReplayEvents(events);
      aggregate.Version = events.Select(x => x.Version).Max();

      return aggregate;
    }

    public async Task RepublishEventAsync()
    {
      var aggregateIds = await eventStore.GetAggregateIdsAsync();

      if (aggregateIds == null || aggregateIds.Any() == false)
      {
        return;
      }

      foreach (var aggregateId in aggregateIds)
      {
        var aggregate = await GetByIdAsync(aggregateId);

        if (aggregate == null || aggregate.Active == false)
        {
          continue;
        }

        var events = await eventStore.GetEventsAsync(aggregateId);
        foreach (var @event in events)
        {
          var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
          await eventProducer.ProduceAsync(topic, @event);
        }
      }

    }

    public async Task SaveAsync(AggregateRoot aggregateRoot)
    {
      await eventStore.SaveEventAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(), aggregateRoot.Version);
      aggregateRoot.MarkChangesAsCommitted();
    }
  }
}
