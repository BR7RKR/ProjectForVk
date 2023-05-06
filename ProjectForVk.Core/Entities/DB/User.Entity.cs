using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectForVk.Core.Entities.DB;

[Table("user")]
public class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }
    
    public required string Login { get; set; }
    
    public required string Password { get; set; }
    
    [JsonPropertyName("created_date")]
    public required DateOnly CreatedDate { get; set; }
    
    [JsonPropertyName("user_group_id")]
    public int UserGroupId { get; set; }
    
    [ForeignKey("UserGroupId")]
    public UserGroupEntity UserGroup { get; set; }
    
    [JsonPropertyName("user_state_id")]
    public int UserStateId { get; set; }
    
    [ForeignKey("UserStateId")]
    public UserStateEntity UserState { get; set; }
}