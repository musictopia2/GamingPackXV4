namespace MonopolyCardGame.Blazor;
public partial class TradeBlazor
{
    [Parameter]
    [EditorRequired]
    public MonopolyCardGamePlayerItem? OppenentUsed { get; set; }
    [Parameter]
    public MonopolyCardGamePlayerItem? SelfPlayer { get; set; }
    [Parameter]
    public EventCallback OnCancelled { get; set; }
    [Parameter]
    public EventCallback<TradeModel> OnTraded { get; set; }
    [Inject]
    private IToast? Toast { get;set; }
    private MonopolyCardGameVMData? _vmData;
    private BasicList<MonopolyCardGameCardInformation> _opponentCards = [];
    private BasicList<MonopolyCardGameCardInformation> _yourCards = [];
    private int _used;
    private BasicList<MonopolyCardGameCardInformation> _proposedYours = [];
    private BasicList<MonopolyCardGameCardInformation> _proposedOpponent = [];
    protected override void OnInitialized()
    {
        _vmData = aa1.Resolver!.Resolve<MonopolyCardGameVMData>();
        _vmData.TempHand1.AutoSelect = EnumHandAutoType.None; //cannot autoselect anymore.
        _opponentCards = OppenentUsed!.TradePile!.HandList.ToBasicList();
        _opponentCards.Reverse();
        _yourCards = SelfPlayer!.TradePile!.HandList.ToBasicList();
        _yourCards.Reverse();
    }
    private bool CanMakeTrade()
    {
        if (SelfPlayer!.TradePile!.HandList.Count < _used)
        {
            return false;
        }
        return true;
    }
    private void ClickCard(MonopolyCardGameCardInformation card)
    {
        _used = _opponentCards.IndexOf(card) + 1;
        if (CanMakeTrade() == false)
        {
            _used = 0;
            Toast!.ShowUserErrorToast("Unable to make trade.  Has to try again");
            return;
        }
        _proposedOpponent = _opponentCards.Take(_used).ToBasicList();
        //_proposedOpponent.Reverse();
        _proposedYours = _yourCards.Take(_used).ToBasicList();
        //_proposedYours.Reverse(); //try this way (?)
    }
    private string StartText => $"Trade With {OppenentUsed!.NickName}";
    private async Task ConfirmTrade()
    {
        TradeModel trade = new(_proposedOpponent, _proposedYours, OppenentUsed!.Id);
        await OnTraded.InvokeAsync(trade);
    }
}