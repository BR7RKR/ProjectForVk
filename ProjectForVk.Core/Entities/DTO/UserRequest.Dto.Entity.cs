using System.Text.Json.Serialization;

namespace ProjectForVk.Core.Entities.DTO;

public sealed class UserRequestDtoEntity
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
}