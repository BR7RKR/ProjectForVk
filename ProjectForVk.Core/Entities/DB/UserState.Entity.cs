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
    
    public static bool operator ==(UserStateEntity a, UserStateEntity b)
    {
        return a?.Equals(b) ?? ReferenceEquals(b, null);
    }
    
    public static bool operator !=(UserStateEntity a, UserStateEntity b)
    {
        return !(a == b);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is UserStateEntity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, (int) Code, Description);
    }
    
    private bool Equals(UserStateEntity other)
    {
        return Id == other.Id && Code == other.Code && Description == other.Description;
    }
}