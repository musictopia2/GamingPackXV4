namespace ThreeLetterFun.Core.Data;
public class TileInformation : SimpleDeckObject, IDeckObject
{
    public char Letter { get; set; }
    public bool IsMoved { get; set; }
    public int Index { get; set; }
    public TileInformation()
    {
        DefaultSize = new SizeF(20, 28);
    }
    public void Reset() { }
    public void Populate(int chosen)
    {
        //decided to do nothing.
        //only needed to implement it because otherwise, can't use the hand view model for the tiles.
    }
}