namespace Hearts.Core.ViewModels;
[InstanceGame]
public partial class HeartsMainViewModel(CommandContainer commandContainer,
        HeartsMainGameClass mainGame,
        HeartsVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<HeartsCardInformation>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = true;
        CreateCommands(CommandContainer);
        base.AdditionalInitialize();
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
        int cardsSelected = viewModel.PlayerHand1!.HowManySelectedObjects;
        if (cardsSelected != 3)
        {
            Toast.ShowUserErrorToast("Must pass 3 cards");
            return;
        }
        var tempList = viewModel.PlayerHand1.ListSelectedObjects().GetDeckListFromObjectList();
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("passcards", tempList);
        }
        await mainGame!.CardsPassedAsync(tempList);
    }
    [Command(EnumCommandCategory.Plain)]
    public async Task MoonAsync(EnumMoonOptions option)
    {
        switch (option)
        {
            case EnumMoonOptions.GiveSelfMinus:
                if (mainGame.BasicData!.MultiPlayer == true)
                {
                    await mainGame.Network!.SendAllAsync("takepointsaway");
                }
                await mainGame!.GiveSelfMinusPointsAsync();
                break;
            case EnumMoonOptions.GiveEverybodyPlus:
                if (mainGame.BasicData!.MultiPlayer == true)
                {
                    await mainGame.Network!.SendAllAsync("givepointseverybodyelse");
                }
                await mainGame!.GiveEverybodyElsePointsAsync();
                break;
            default:
                throw new CustomBasicException("Not Supported");
        }
    }
}