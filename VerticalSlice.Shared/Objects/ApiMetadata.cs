namespace VerticalSlice.Shared.Objects;

public class ApiMetadata : Dictionary<string, string>
{
    public string Timestamp { get; set; } = DateTime.UtcNow.ToString("o");
    public string? RequestId { get; set; } = Guid.NewGuid().ToString();
}