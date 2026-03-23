namespace Infrastructure.Assembly.Internal;

internal class AssemblyState
{
    public string CurrentOperation { get; set; } = string.Empty;
    public double Progress { get; set; }
    public Queue<string> OperationQueue { get; set; } = new();
}
