namespace SolidMatrix.Toolkits.QrSheets;

public class UniqueGenerator<T> : IDisposable
{
    private static readonly string DefaultHistoryFilename = "unique-generator-history.txt";
    private readonly HashSet<string> _history;
    private readonly string _historyDirectoryPath;
    private readonly string _historyFilename;
    private string HistoryPath => Path.Combine(_historyDirectoryPath, _historyFilename);

    public UniqueGenerator(string historyDirectoryPath, string historyFilename)
    {
        (_historyDirectoryPath, _historyFilename) = (historyDirectoryPath, historyFilename);
        if (File.Exists(HistoryPath))
        {
            _history = File.ReadAllLines(HistoryPath).ToHashSet();
        }
        else
        {
            _history = new();
        }

    }
    public UniqueGenerator(string historyDirectoryPath) : this(historyDirectoryPath, DefaultHistoryFilename) { }
    public UniqueGenerator() : this(Environment.CurrentDirectory) { }


    private bool Check(T? value)
    {
        if (value == null) return false;
        var valueString = value.ToString();
        if (valueString == null) return false;
        return !_history.Contains(valueString);
    }

    public T Next(Func<Func<T, bool>, T?> generator)
    {
        T? value = generator(v =>
        {
            if (v == null) return false;
            var vs = v.ToString();
            if (vs == null) return false;
            return !_history.Contains(vs);
        });

        if (!Check(value)) throw new Exception("value not available");

        _history.Add(value!.ToString()!);

        return value;
    }

    public void Dispose()
    {
        File.WriteAllLines(HistoryPath, _history);
    }
}
