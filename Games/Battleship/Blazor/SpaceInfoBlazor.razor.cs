namespace Battleship.Blazor;
public partial class SpaceInfoBlazor
{
    [CascadingParameter]
    public string TargetHeight { get; set; } = "";
    [Parameter]
    public ShipInfoCP? Ship { get; set; }
    [CascadingParameter]
    public BattleshipMainViewModel? DataContext { get; set; }
    private static string PieceColor(PieceInfoCP piece)
    {
        if (piece.DidHit)
        {
            return cs1.Red.ToWebColor;
        }
        return cs1.Gray.ToWebColor;
    }
    private string ShipColor()
    {
        if (DataContext!.VMData.ShipSelected == Ship!.ShipCategory)
        {
            return cs1.LimeGreen.ToWebColor;
        }
        return cs1.Orange.ToWebColor;
    }
    private void ChooseShip()
    {
        if (DataContext!.CanChooseShip() == false || DataContext.CommandContainer.IsExecuting == true)
        {
            return;
        }
        DataContext.ChooseShip(Ship!.ShipCategory);
        DataContext.CommandContainer.UpdateAll(); //has to manually update all this time.
    }
}