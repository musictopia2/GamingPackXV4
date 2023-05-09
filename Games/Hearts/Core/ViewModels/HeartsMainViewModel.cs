namespace Hearts.Core.ViewModels;
[InstanceGame]
public partial class HeartsMainViewModel : BasicCardGamesVM<HeartsCardInformation>
{
    private readonly HeartsMainGameClass _mainGame; //if we don't need, delete.
    private readonly HeartsVMData _model;
    private readonly IToast _toast;

    public HeartsMainViewModel(CommandContainer commandContainer,
        HeartsMainGameClass mainGame,
        HeartsVMData viewModel,
        BasicData basicData,
        TestOptions test,
        IGamePackageResolver resolver,
        HeartsGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast
        )
        : base(commandContainer, mainGame, viewModel, basicData, test, resolver, aggregator, toast)
    {
        _mainGame = mainGame;
        _model = viewModel;
        _toast = toast;
        _model.Deck1.NeverAutoDisable = true;
        CreateCommands(commandContainer);
    }
    partial void CreateCommands(CommandContainer command);
    //anything else needed is here.
    protected override bool CanEnableDeck()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }

    protected override bool CanEnablePile1()
    {
        //todo:  decide whether to enable deck.
        return false; //otherwise, can't compile.
    }

    protected override async Task ProcessDiscardClickedAsync()
    {
        //if we have anything, will be here.
        await Task.CompletedTask;
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    [Command(EnumCommandCategory.Game)]
    public async Task PassAsync()
    {
        int cardsSelected = _model.PlayerHand1!.HowManySelectedObjects;
        if (cardsSelected != 3)
        {
            _toast.ShowUserErrorToast("Must pass 3 cards");
            return;
        }
        var tempList = _model.PlayerHand1.ListSelectedObjects().GetDeckListFromObjectList();
        if (_mainGame.BasicData!.MultiPlayer == true)
        {
            await _mainGame.Network!.SendAllAsync("passcards", tempList);
        }
        await _mainGame!.CardsPassedAsync(tempList);
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task MoonAsync(EnumMoonOptions option)
    {
        switch (option)
        {
            case EnumMoonOptions.GiveSelfMinus:
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    await _mainGame.Network!.SendAllAsync("takepointsaway");
                }
                await _mainGame!.GiveSelfMinusPointsAsync();
                break;
            case EnumMoonOptions.GiveEverybodyPlus:
                if (_mainGame.BasicData!.MultiPlayer == true)
                {
                    await _mainGame.Network!.SendAllAsync("givepointseverybodyelse");
                }
                await _mainGame!.GiveEverybodyElsePointsAsync();
                break;
            default:
                throw new CustomBasicException("Not Supported");
        }
    }
}