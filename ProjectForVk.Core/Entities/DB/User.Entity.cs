using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectForVk.Core.Entities.DB;

[Table("user")]
public sealed class UserEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public required int Id { get; set; }
    
    [Column("login")]
    public required string Login { get; set; }
    
    [Column("password")]
    public required string Password { get; set; }
    
    [Column("created_date")]
    [JsonPropertyName("created_date")]
    public required DateOnly CreatedDate { get; set; }
    
    [Column("user_group_id")]
    [JsonPropertyName("user_group_id")]
    public int UserGroupId { get; set; }
    
    [ForeignKey("UserGroupId")]
    public UserGroupEntity UserGroup { get; set; }
    
    [Column("user_state_id")]
    [JsonPropertyName("user_state_id")]
    public int UserStateId { get; set; }
    
    [ForeignKey("UserStateId")]
    public UserStateEntity UserState { get; set; }
}