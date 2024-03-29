namespace Mancala.Blazor;
public partial class MancalaBoardBlazor
{
    [Parameter]
    public GameBoardVM? DataContext { get; set; }
    private int StrokeWidth(SpaceInfo space)
    {
        int index = DataContext!.GetIndex(space);
        if (index == DataContext.GameData.SpaceSelected || index == DataContext.GameData.SpaceStarted)
        {
            return 4;
        }
        return 2;
    }
    private string StrokeColor(SpaceInfo space)
    {
        int index = DataContext!.GetIndex(space);
        if (index == DataContext.GameData.SpaceSelected)
        {
            return cc1.Red;
        }
        if (index == DataContext.GameData.SpaceStarted)
        {
            return cc1.Green;
        }
        return cc1.Black;
    }
}