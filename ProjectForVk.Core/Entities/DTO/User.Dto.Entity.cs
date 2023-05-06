using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Core.Entities.DTO;

[Serializable]
public sealed class UserDtoEntity
{
    public required int Id { get; set; }
    
    public required string Login { get; set; }
    
    public required string Password { get; set; }
    
    [JsonPropertyName("created_date")]
    public required DateOnly CreatedDate { get; set; }
    
    [JsonPropertyName("user_group_id")]
    public int UserGroupId { get; set; }

    [JsonPropertyName("user_state_id")]
    public int UserStateId { get; set; }
}