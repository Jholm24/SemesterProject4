namespace Infrastructure.Assembly.Internal;

internal class AssemblyConfig
{
    public IList<string> AvailableTools { get; set; } = new List<string> { "gripper", "screwdriver" };
    public int CycleTimeSeconds { get; set; } = 30;
}
