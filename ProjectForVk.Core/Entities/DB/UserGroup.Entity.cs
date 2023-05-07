using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ProjectForVk.Core.Entities.Types;

namespace ProjectForVk.Core.Entities.DB;

[Table("user_group")]
public sealed class UserGroupEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Column("id")]
    public required int Id { get; set; }
    
    [Column("code")]
    public required GroupCodeType Code { get; set; }
    
    [Column("description")]
    public required string Description { get; set; }
    
    public static bool operator ==(UserGroupEntity a, UserGroupEntity b)
    {
        return a?.Equals(b) ?? ReferenceEquals(b, null);
    }
    
    public static bool operator !=(UserGroupEntity a, UserGroupEntity b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is UserGroupEntity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int) Code, Description);
    }
    
    private bool Equals(UserGroupEntity other)
    {
        return Id == other.Id && Code == other.Code && Description == other.Description;
    }
}