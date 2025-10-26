namespace VerticalSlice.Shared.Abstractions;

public interface IAuditable
{
    public int CreatedBy { get; set; }
    public DateTime CreatedTimeStamp { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedTimeStamp { get; set; }
}