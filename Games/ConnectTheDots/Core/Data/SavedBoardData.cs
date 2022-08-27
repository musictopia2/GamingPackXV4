namespace ConnectTheDots.Core.Data;
public class SavedBoardData
{
    public BasicList<bool> LineList { get; set; } = new(); // this represents the lines
    public BasicList<bool> DotList { get; set; } = new(); // to represent whether the dot was selected or not.
    public int PreviousColumn { get; set; }
    public int PreviousRow { get; set; }
    public int PreviousLine { get; set; }
    public BasicList<int> SquarePlayerList { get; set; } = new();
}