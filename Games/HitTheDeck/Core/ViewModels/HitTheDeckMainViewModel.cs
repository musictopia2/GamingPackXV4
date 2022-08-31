namespace HitTheDeck.Core.ViewModels;
[InstanceGame]
public partial class HitTheDeckMainViewModel : BasicCardGamesVM<HitTheDeckCardInformation>
{
    private readonly HitTheDeckMainGameClass _mainGame;
    private readonly HitTheDeckVMData _model;
    private readonly HitTheDeckGameContainer _gameContainer;
    private readonly IToast _toast;
    public HitTheDeckMainViewModel(CommandContainer commandContainer,
        HitTheDeckMainGameClass mainGame,
        HitTheDeckVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        HitTheDeckGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _gameContainer = gameContainer;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    protected override bool CanEnableDeck()
    {
        if (NeedsSpecial == true)
        {
            return false;
        }
        if (_mainGame!.SingleInfo!.MainHandList.Any(items => _mainGame.CanPlay(items.Deck)) && _mainGame.Test!.AllowAnyMove == false)
        {
            return false;
        }
        return !_gameContainer.AlreadyDrew;
    }
    protected override bool CanEnablePile1()
    {
        return !NeedsSpecial;
    }
    protected override async Task ProcessDiscardClickedAsync()
    {
        int newDeck = _model.PlayerHand1!.ObjectSelected();
        if (newDeck == 0)
        {
            _toast.ShowUserErrorToast("Sorry, must select a card first");
            return;
        }
        if (_mainGame!.CanPlay(newDeck) == false)
        {
            _toast.ShowUserErrorToast("Illegal move");
            return;
        }
        await _mainGame.ProcessPlayAsync(newDeck);
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        var thisCard = _model.Pile1!.GetCardInfo();
        if (thisCard.Instructions == EnumInstructionList.Flip || thisCard.Instructions == EnumInstructionList.Cut)
        {
            return false;
        }
        if (_mainGame!.SingleInfo!.MainHandList.Any(items => _mainGame.CanPlay(items.Deck)))
        {
            return false;
        }
        return _gameContainer.AlreadyDrew;
    }
    private bool NeedsSpecial
    {
        get
        {
            var thisCard = _model.Pile1!.GetCardInfo();
            return thisCard.Instructions == EnumInstructionList.Cut || thisCard.Instructions == EnumInstructionList.Flip;
        }
    }
    public bool CanCut()
    {
        var thisCard = _model.Pile1!.GetCardInfo();
        return thisCard.Instructions == EnumInstructionList.Cut;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task CutAsync()
    {
        await _mainGame!.CutDeckAsync();
    }
    public bool CanFlip()
    {
        var thisCard = _model.Pile1!.GetCardInfo();
        return thisCard.Instructions == EnumInstructionList.Flip;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task FlipAsync()
    {
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("flipdeck");
        }
        await _mainGame!.FlipDeckAsync();
    }
}