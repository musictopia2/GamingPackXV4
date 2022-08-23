namespace BasicGameFrameworkLibrary.Core.MultiplePilesObservable;

public class BasicPileInfo<D> where D : IDeckObject, new()
{
    public BasicPileInfo() { }
    public bool IsSelected { get; set; }
    public bool IsEnabled { get; set; } = true;
    public string Text { get; set; } = "";
    public DeckRegularDict<D> ObjectList { get; set; } = new();
    public DeckRegularDict<D> TempList = new();
    public bool Rotated { get; set; }
    public D ThisObject { get; set; } = new();
    public bool Visible { get; set; } = true;
    public int Column { get; set; }
    public int Row { get; set; }
}