using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectForVk.Core.Entities.DB;

public class UserEntity
{
    [Key]
    public required string Id { get; set; }
    
    public required string Login { get; set; }
    
    public required string Password { get; set; }
    
    public required DateOnly CreatedDate { get; set; }
    
    public string UserGroupId { get; set; }
    
    [ForeignKey("UserGroupId")]
    public UserGroupEntity UserGroup { get; set; }

    public string UserStateId { get; set; }
    
    [ForeignKey("UserStateId")]
    public UserStateEntity UserState { get; set; }
}