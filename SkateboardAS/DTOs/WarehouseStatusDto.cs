namespace SkateboardAS.DTOs;

public class WarehouseStatusDto
{
    public string Id { get; set; } = string.Empty;
    public int TotalSlots { get; set; }
    public int OccupiedSlots { get; set; }
    public string State { get; set; } = string.Empty;
}
