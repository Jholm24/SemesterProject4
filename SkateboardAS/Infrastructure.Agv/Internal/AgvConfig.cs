namespace Infrastructure.Agv.Internal;

internal class AgvConfig
{
    public double MaxSpeed { get; set; } = 1.0;
    public double MaxPayloadKg { get; set; } = 50.0;
    public string HomePosition { get; set; } = "storage";
}
