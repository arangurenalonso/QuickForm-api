namespace QuickForm.Common.Domain;


public sealed record FindResult<TFound, TMissing>(
    IReadOnlyList<TFound> Found,
    IReadOnlyList<TMissing> Missing)
{
    public int FoundCount => Found.Count;
    public int MissingCount => Missing.Count;
    public bool IsAllFound => MissingCount == 0;
    public bool HasMissing => MissingCount > 0;
    public void Deconstruct(out IReadOnlyList<TFound> found, out IReadOnlyList<TMissing> missing)
    {
        found = Found;
        missing = Missing;
    }

    public static FindResult<TFound, TMissing> From(
        IEnumerable<TFound> found,
        IEnumerable<TMissing> requested,
        Func<TFound, TMissing> foundKeySelector,
        IEqualityComparer<TMissing>? comparer = null)
    {
        ArgumentNullException.ThrowIfNull(found);
        ArgumentNullException.ThrowIfNull(requested);
        ArgumentNullException.ThrowIfNull(foundKeySelector);


        comparer ??= EqualityComparer<TMissing>.Default;

        var foundList = found.ToList();

        var foundKeys = new HashSet<TMissing>(foundList.Select(foundKeySelector), comparer);

        var missing = requested.Where(r => !foundKeys.Contains(r)).ToList();

        return new FindResult<TFound, TMissing>(
            Found: foundList.ToList(),
            Missing: missing
        );
    }

    public static FindResult<TFound, TMissing> Create(
        IEnumerable<TFound> found,
        IEnumerable<TMissing> missing)
        => new(found?.ToList() ?? new List<TFound>(),
               missing?.ToList() ?? new List<TMissing>());
}

