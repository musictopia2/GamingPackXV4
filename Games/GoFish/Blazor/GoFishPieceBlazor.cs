namespace GoFish.Blazor;
public class GoFishPieceBlazor : NumberPiece
{
    [Parameter]
    public EnumRegularCardValueList Value { get; set; }
    protected override void OriginalSizeProcesses()
    {
        MainGraphics!.OriginalSize = new SizeF(45, 45);
        base.OriginalSizeProcesses();
    }
    protected override string GetValueToPrint()
    {
        if (Value == EnumRegularCardValueList.Jack)
        {
            return "J";
        }
        if (Value == EnumRegularCardValueList.Queen)
        {
            return "Q";
        }
        if (Value == EnumRegularCardValueList.King)
        {
            return "K";
        }
        if (Value == EnumRegularCardValueList.None)
        {
            return "";
        }
        if (Value == EnumRegularCardValueList.LowAce || Value == EnumRegularCardValueList.HighAce)
        {
            return "A";
        }
        return Value.Value.ToString(); //hopefully that works (?)
    }
}
