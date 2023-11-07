using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace EnumGenerator;

[MemoryDiagnoser(false)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
public class Benchmarks
{
    [Benchmark]
    public string EnumToString()
    {
        return Status2.ToDo.ToString();
    }

    [Benchmark]
    public string IntellenumToString()
    {
        return Status.ToDo.ToString();
    }
    
    [Benchmark]
    public bool EnumIsDefined()
    {
        return Enum.IsDefined(typeof(Status2), (Status2)1);
    }
    
    [Benchmark]
    public bool IntellenumIsDefined()
    {
        return Status.IsDefined(1);
    }
    
    [Benchmark]
    public (bool, Status2) EnumTryParseFromName()
    {
        var couldParse = Enum.TryParse<Status2>("ToDo", out var value);
        //var couldParse2 = Enum.TryParse("ToDo", false, out Status2 value);
        return (couldParse, value);
    }
    
    [Benchmark]
    public (bool, Status) IntellenumTryParseFromName()
    {
        var couldParse = Status.TryFromName("To do", out var value);
        return (couldParse, value);
    }
    
    [Benchmark]
    public (bool, Status2) EnumTryParseFromValue()
    {
        var couldParse = Enum.TryParse<Status2>("1", out var value);
        //var couldParse2 = Enum.TryParse("1", false, out Status2 value);
        return (couldParse, value);
    }
    
    [Benchmark]
    public (bool, Status) IntellenumTryParseFromValue()
    {
        var couldParse = Status.TryFromValue(1, out var value);
        return (couldParse, value);
    }
}