namespace FrozenCollections.StringComparers;

internal abstract class PartialStringComparerBase : StringComparerBase
{
    public int Index;
    public int Count;

    public abstract bool EqualsPartial(string? x, string? y);
}
