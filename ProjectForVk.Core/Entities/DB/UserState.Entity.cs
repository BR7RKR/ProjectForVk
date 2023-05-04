using System.ComponentModel.DataAnnotations;
using ProjectForVk.Core.Entities.Types;

namespace ProjectForVk.Core.Entities.DB;

public class UserStateEntity
{
    [Key]
    public required string Id { get; set; }
    
    public required StateCodeType Code { get; set; }
    
    public required string Description { get; set; }
}