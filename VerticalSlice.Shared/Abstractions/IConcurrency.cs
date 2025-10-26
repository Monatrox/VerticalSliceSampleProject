namespace VerticalSlice.Shared.Abstractions;

public interface IConcurrency
{
    public byte[] RowVersion { get; set; }
}