using CQRS.Core.Events;
using Post.Common.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Post.Query.Infrastructure.Converters
{
  public class EventJsonConverter : JsonConverter<BaseEvent>
  {
    public override bool CanConvert(Type typeToConvert)
    {
      return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
    }
    public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      if (!JsonDocument.TryParseValue(ref reader, out var doc))
      {
        throw new JsonException($"Failed to parse {nameof(JsonDocument)} ");
      }
      if (!doc.RootElement.TryGetProperty("Type", out var type))
      {
        throw new JsonException("Coudn't detect the Type");
      }

      var typeDiscriminator = type.GetString();
      var json = doc.RootElement.GetRawText();

      return typeDiscriminator switch
      {
        nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json),
        nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(json),
        nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json),
        nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json),
        nameof(CommentUpdatedEvent) => JsonSerializer.Deserialize<CommentUpdatedEvent>(json),
        nameof(DeletePostEvent) => JsonSerializer.Deserialize<DeletePostEvent>(json),
        nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json),
        _=> throw new JsonException($"{typeDiscriminator} is not implemented yet!")
      };
    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
      throw new NotImplementedException();
    }
  }
}
