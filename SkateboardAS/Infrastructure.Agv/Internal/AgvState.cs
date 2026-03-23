namespace Infrastructure.Agv.Internal;

internal class AgvState
{
    public string Position { get; set; } = string.Empty;
    public double Battery { get; set; }
    public string CurrentRoute { get; set; } = string.Empty;
}
