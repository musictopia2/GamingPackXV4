namespace BasicGameFrameworkLibrary.Core.MultiplePilesObservable;

[UseStateChanged]
public partial class BasicPileInfo<D> where D : IDeckObject, new()
{
    public BasicPileInfo() { }
    [StateProperty]
    public partial bool IsSelected { get; set; }

    partial void OnIsSelectedChanged(bool value)
    {
        ThisObject.IsSelected = value;
    }

    [StateProperty]
    public partial bool IsEnabled { get; set; } = true;
    [StateProperty]
    public partial string Text { get; set; } = "";
    public DeckRegularDict<D> ObjectList { get; set; } = new();
    public DeckRegularDict<D> TempList = new();
    public bool Rotated { get; set; }
    public D ThisObject { get; set; } = new();
    public bool Visible { get; set; } = true;
    public int Column { get; set; }
    public int Row { get; set; }
}