using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ProjectForVk.Infrastructure.Database.DateOnlySupport;

internal sealed class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(
        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime))
    { }
}