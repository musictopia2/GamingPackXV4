namespace Racko.Blazor;
public partial class RackoUI
{
    [CascadingParameter]
    public RackoMainViewModel? DataContext { get; set; }
    [CascadingParameter]
    public RackoGameContainer? GameContainer { get; set; }
    [CascadingParameter]
    public int TargetHeight { get; set; } = 15;
    private string RealHeight => $"{TargetHeight}vh";
    private bool IsEnabled
    {
        get
        {
            if (DataContext!.CommandContainer.IsExecuting)
            {
                return false;
            }
            return DataContext.CanPlayOnPile;
        }
    }
    private BasicList<RackoCardInformation>? _cardList;
    private BasicList<int>? _otherList;
    protected override void OnParametersSet()
    {
        var selfPlayer = GameContainer!.PlayerList!.GetSelf();
        _cardList = selfPlayer.MainHandList.ToBasicList();
        _cardList.Reverse();
        int x;
        int starts = GameContainer.PlayerList.Count + 2;
        int diffs = starts;
        _otherList = new();
        for (x = 1; x <= 10; x++)
        {
            _otherList.Add(starts);
            starts += diffs;
        }
        _otherList.Reverse();
        base.OnParametersSet();
    }
    private string GetLabel(RackoCardInformation card)
    {
        return _otherList![_cardList!.IndexOf(card)].ToString();
    }
    private async Task ClickedRowAsync(RackoCardInformation row)
    {
        if (IsEnabled == false)
        {
            return;
        }
        await DataContext!.CommandContainer.ProcessCustomCommandAsync(DataContext.PlayOnPileAsync, row);
    }
}