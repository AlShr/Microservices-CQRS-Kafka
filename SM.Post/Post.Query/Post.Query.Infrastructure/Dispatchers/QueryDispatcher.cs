using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.Dispatchers
{
  public class QueryDispatcher : IQueryDispatcher<PostEntity>
  {
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> handlers = new();

    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
    {
      if (handlers.ContainsKey(typeof(TQuery)))
      {
        throw new IndexOutOfRangeException("You can not register the same query handler twice");
      }

      handlers.Add(typeof(TQuery), x => handler((TQuery)x));
    }

    public async Task<List<PostEntity>> SendAsync(BaseQuery query)
    {
      if (handlers.TryGetValue(query.GetType(), out Func<BaseQuery, Task<List<PostEntity>>> handler))
      {
        return await handler(query);
      }

      throw new ArgumentNullException(nameof(handler), "No query handler was registered");
    }
  }
}
