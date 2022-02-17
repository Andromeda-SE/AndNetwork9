using And9.Lib.Models.Abstractions.Interfaces;

namespace And9.Service.Award.Abstractions.Interfaces;

public interface IAward : IBaseAward, IComparable<IAward>, IComparable, IId
{
    public DateOnly Date { get; }
    public int? AutomationTag { get; }
    public int? GaveById { get; }

    int IComparable.CompareTo(object? obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        if (ReferenceEquals(this, obj)) return 0;
        return obj is IAward other
            ? CompareTo(other)
            : throw new ArgumentException($"Object must be of type {nameof(IAward)}");
    }

    int IComparable<IAward>.CompareTo(IAward? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        int typeComparison = Type.CompareTo(other.Type);
        if (typeComparison != 0) return typeComparison;
        return Date.CompareTo(other.Date);
    }
}