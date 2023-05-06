using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectForVk.Core.Entities.Types;

namespace ProjectForVk.Core.Entities.DB;

[Table("user_group")]
public class UserGroupEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required int Id { get; set; }
    
    [RegularExpression(@"^(Admin|User)$")]
    public required string Code { get; set; }
    
    public required string Description { get; set; }
}