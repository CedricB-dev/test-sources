using Intellenum;

[Intellenum]
public partial class Status
{
    public static readonly Status ToDo = new("To do", 1);
    public static readonly Status InProgress = new("In Progress", 2);
    public static readonly Status Done = new(3);
}

public enum Status2
{
    ToDo = 1,
    InProgress = 2,
    Done = 3
}
