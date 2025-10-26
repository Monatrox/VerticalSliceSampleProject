using System.ComponentModel;

namespace VerticalSlice.Shared.Abstractions;

public interface IHasActive
{
    [DefaultValue(true)]
    public bool Active { get; set; }
}