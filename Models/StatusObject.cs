using desseract.Enums;

namespace desseract.Models;

public class StatusObject
{
    public EngineStatus Status { get; set; }
    public string? Output { get; set; }
}