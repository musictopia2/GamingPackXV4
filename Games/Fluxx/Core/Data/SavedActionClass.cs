namespace Fluxx.Core.Data;
public class SavedActionClass
{
    public int SelectedIndex { get; set; }
    public BasicList<int> TempHandList { get; set; } = [];
    public BasicList<PreviousCard> PreviousList { get; set; } = [];
    public BasicList<int> TempDiscardList { get; set; } = [];
}