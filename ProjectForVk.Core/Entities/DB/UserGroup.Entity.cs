using System.ComponentModel.DataAnnotations;
using ProjectForVk.Core.Entities.Types;

namespace ProjectForVk.Core.Entities.DB;

public class UserGroupEntity
{
    [Key]
    public required string Id { get; set; }
    
    public required GroupCodeType Code { get; set; }
    
    public required string Description { get; set; }
}