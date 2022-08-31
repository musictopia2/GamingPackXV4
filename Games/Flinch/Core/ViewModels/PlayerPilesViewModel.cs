namespace Flinch.Core.ViewModels;
[InstanceGame]
public class PlayerPilesViewModel : ScreenViewModel, IBlankGameVM
{
    public readonly FlinchVMData Model;
    private readonly FlinchMainGameClass _mainGame;
    private readonly IToast _toast;
    public PlayerPilesViewModel(CommandContainer commandContainer, 
        FlinchGameContainer gameContainer, 
        FlinchVMData model, 
        FlinchMainGameClass mainGame,
        IToast toast,
        IEventAggregator aggregator) : base(aggregator)
    {
        gameContainer.SingleInfo = gameContainer.PlayerList!.GetWhoPlayer();
        CommandContainer = commandContainer;
        GameContainer = gameContainer;
        Model = model;
        _mainGame = mainGame;
        _toast = toast;
        Model.StockPile.ClearCards();
        gameContainer.SingleInfo!.StockList.ForEach(x =>
        {
            model.StockPile.AddCard(x);
        });
        Model.DiscardPiles = new DiscardPilesVM<FlinchCardInformation>(commandContainer);
        Model.DiscardPiles.Init(HowManyDiscards);
        if (gameContainer.SingleInfo!.DiscardList.Count > 0)
        {
            model.DiscardPiles!.PileList!.ReplaceRange(gameContainer.SingleInfo.DiscardList);
        }
        Model.DiscardPiles.PileClickedAsync += DiscardPiles_PileClickedAsync;
        Model.StockPile!.StockClickedAsync += StockPile_StockClickedAsync;
    }
    protected override Task TryCloseAsync()
    {
        Model.DiscardPiles!.PileClickedAsync -= DiscardPiles_PileClickedAsync;
        Model.StockPile!.StockClickedAsync -= StockPile_StockClickedAsync;
        return base.TryCloseAsync();
    }
    public CommandContainer CommandContainer { get; set; }
    public FlinchGameContainer GameContainer { get; }
    private async Task DiscardPiles_PileClickedAsync(int index, BasicPileInfo<FlinchCardInformation> thisPile)
    {
        int playerDeck = Model!.PlayerHand1!.ObjectSelected();
        if (playerDeck > 0)
        {
            if (_mainGame!.SaveRoot!.GameStatus == EnumStatusList.FirstOne && _mainGame.SaveRoot.PlayerFound == 0)
            {
                _toast.ShowUserErrorToast("Sorry; cannot discard one because you need either a one or else pass on this turn");
                _mainGame.UnselectAllCards();
                return;
            }
            if (Model.DiscardPiles!.CanAddToDiscardFlinch(index) == false)
            {
                _toast.ShowUserErrorToast("Sorry, cannot discard here because there is an empty one that has to be used first");
                return;
            }
            await _mainGame!.AddToDiscardAsync(index, playerDeck);
            return;
        }
        if (Model.DiscardPiles!.HasCard(index) == false)
        {
            return;
        }
        if (Model.DiscardPiles.PileList![index].IsSelected == true)
        {
            Model.DiscardPiles.PileList[index].IsSelected = false;
            return;
        }
        _mainGame!.UnselectAllCards();
        Model.DiscardPiles.SelectUnselectSinglePile(index);
    }
    private Task StockPile_StockClickedAsync()
    {
        int nums = Model.StockPile!.CardSelected();
        _mainGame!.UnselectAllCards();
        if (nums > 0)
        {
            Model.StockPile.UnselectCard();
            return Task.CompletedTask;
        }
        Model.StockPile.SelectCard();
        return Task.CompletedTask;
    }
}
