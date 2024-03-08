namespace DealCardGame.Blazor;
public partial class CompletePlayerSetsComponent
{
    [Parameter]
    [EditorRequired]
    public BasicList<DealCardGamePlayerItem> Players { get; set; } = [];
    private static string Columns => gg1.RepeatSpreadOut(10);
    private static string Rows => gg1.RepeatSpreadOut(5);
    private static EnumColor GetColor(int column) => EnumColor.FromValue(column);
    [Inject]
    private IToast? Toast { get; set; }
    private void PrivateSetClicked(SetPlayerModel set)
    {
        var player = Players[set.PlayerId - 1]; //somehow 0 based.
        Toast!.ShowInfoToast($"Clicked On Color {set.Color} with nick name of {player.NickName}");
    }
    private bool HasHotel(int column, int playerId)
    {
        EnumColor color = GetColor(column);
        DealCardGamePlayerItem playerUsed = Players[playerId - 1]; //because 0 based.
        var list = playerUsed.SetData.GetCards(color);
        return list.HasHotel();
    }
    private bool HasHouse(int column, int playerId)
    {
        EnumColor color = GetColor(column);
        DealCardGamePlayerItem playerUsed = Players[playerId - 1]; //because 0 based.
        var list = playerUsed.SetData.GetCards(color);
        return list.HasHouse();
    }
    private int SetsCompleted(int column, int playerId)
    {
        EnumColor color = GetColor(column);
        DealCardGamePlayerItem playerUsed = Players[playerId - 1]; //because 0 based.
        var list = playerUsed.SetData.GetCards(color);
        return list.HowManyCompleted();
    }
    private int RentOwed(int column, int playerId, bool hasHouse, bool hasHotel)
    {
        EnumColor color = GetColor(column);
        DealCardGamePlayerItem playerUsed = Players[playerId - 1]; //because 0 based.
        var list = playerUsed.SetData.GetCards(color);
        return list.RentForSet(color, hasHouse, hasHotel);
    }



    //private static bool HasTestHotelOrHouse(int column)
    //{
    //    EnumColor color = GetColor(column);
    //    if (color == EnumColor.Black || color == EnumColor.Lime)
    //    {
    //        return false;
    //    }
    //    return true;
    //}
}