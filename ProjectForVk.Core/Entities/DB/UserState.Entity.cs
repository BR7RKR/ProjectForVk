using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectForVk.Core.Entities.Types;

namespace ProjectForVk.Core.Entities.DB;

[Table("user_state")]
public sealed class UserStateEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public required int Id { get; set; }
    
    [Column("code")]
    public required StateCodeType Code { get; set; }
    
    [Column("description")]
    public required string Description { get; set; }
}