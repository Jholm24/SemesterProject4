namespace Core.Metadata;

public interface IComponentMetadata
{
    string Name { get; }
    string Version { get; }
    string Protocol { get; }
    string MachineType { get; }
    string Description { get; }
    string Icon { get; }
    int Priority { get; }
}
