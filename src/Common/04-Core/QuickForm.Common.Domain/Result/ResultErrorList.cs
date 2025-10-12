namespace QuickForm.Common.Domain;
public class ResultErrorList
{
    private readonly List<ResultError> _errors;

    public ResultErrorList()
    {
        _errors = new List<ResultError>();
    }

    public ResultErrorList(List<ResultError> errors)
    {
        _errors = errors ?? new List<ResultError>();
    }

    public ResultErrorList(List<ResultErrorList> errors)
    {
        _errors = errors.SelectMany(e => e.ToList()).Where(x => x != ResultError.None).ToList();

    }
    public ResultErrorList(ResultError error)
    {
        _errors = new List<ResultError> { error };
    }
    public void Add(ResultError error)
    {
        if (error != null && error != ResultError.None)
        {
            _errors.Add(error);
        }
    }
    public void SetField(string field)
    {
        foreach (var error in _errors)
        {
            error.SetField(field);
        }
    }


    public List<ResultError> ToList() => _errors;

    public int Count => _errors.Count;

    public ResultError this[int index] => _errors[index];
    public override string ToString()
       => string.Join("; ",
           _errors
               .Where(e => e is not null && e != ResultError.None)
               .Select(e => e!.ToString()));

}
