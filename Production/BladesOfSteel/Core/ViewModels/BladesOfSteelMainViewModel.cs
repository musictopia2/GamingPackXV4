namespace BladesOfSteel.Core.ViewModels;
[InstanceGame]
public partial class BladesOfSteelMainViewModel(CommandContainer commandContainer,
        BladesOfSteelMainGameClass mainGame,
        BladesOfSteelVMData viewModel,
        BasicData basicData,
        IGamePackageResolver resolver,
        BladesOfSteelGameContainer gameContainer,
        IEventAggregator aggregator,
        IToast toast) : BasicCardGamesVM<RegularSimpleCard>(commandContainer, mainGame, viewModel, basicData, resolver, aggregator, toast)
{
    
    protected override void AdditionalInitialize()
    {
        viewModel.Deck1.NeverAutoDisable = false;
        viewModel.PlayerHand1.Maximum = 6;
        viewModel.PlayerHand1.AutoSelect = EnumHandAutoType.SelectAsMany;
        viewModel.MainDefense1.SendEnableProcesses(this, () => mainGame.SaveRoot.PlayOrder.OtherTurn > 0);
        viewModel.YourAttackPile.SendEnableProcesses(this, () => CanEnablePiles(false));
        viewModel.YourDefensePile.SendEnableProcesses(this, () => CanEnablePiles(true));
        CreateCommands(CommandContainer);
        LoadAttackCommands();
        base.AdditionalInitialize();
    }
    partial void CreateCommands(CommandContainer command);
    private bool CanEnablePiles(bool isDefense)
    {
        if (mainGame!.SaveRoot!.PlayOrder.OtherTurn > 0)
        {
            return isDefense;
        }
        return !viewModel.MainDefense1!.HasCards;
    }
    private void LoadAttackCommands()
    {
        viewModel.MainDefense1.BoardClickedAsync = MainDefense1_BoardClickedAsync;
        viewModel.YourAttackPile.BoardClickedAsync = YourAttackPile_BoardClickedAsync;
        viewModel.YourDefensePile.BoardClickedAsync = YourDefensePile_BoardClickedAsync;
        viewModel.YourDefensePile.ObjectClickedAsync = YourDefensePile_ObjectClickedAsync;
    }
    protected override bool CanEnableDeck()
    {
        return mainGame.SaveRoot.IsFaceOff; //only case you can enable is during faceoffs.  try that way.
    }

    protected override bool CanEnablePile1()
    {
        return !viewModel.MainDefense1!.HasCards;
    }

    protected override async Task ProcessDiscardClickedAsync()
    {
        if (mainGame!.SingleInfo!.MainHandList.Count == 0 && mainGame.SingleInfo.DefenseList.Count == 0)
        {
            throw new CustomBasicException("There are no cards from hand or defense list.  Therfore; should have disabled the pile");
        }
        if (mainGame.SingleInfo.DefensePile!.HandList.HasSelectedObject() == true)
        {
            if (mainGame.SingleInfo.DefenseList.All(items => items.IsSelected == false))
            {
                Toast.ShowUserErrorToast("If you choose one card from defense, you must choose all cards");
                return;
            }
            if (mainGame.BasicData!.MultiPlayer == true)
            {
                await mainGame.Network!.SendAllAsync("throwawaydefense");
            }
            await mainGame!.ThrowAwayDefenseCardsAsync();
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("throwawayallcardsfromhand");
        }
        await mainGame!.ThrowAwayAllCardsFromHandAsync();
    }
    public override bool CanEnableAlways()
    {
        return true;
    }
    public override bool CanEndTurn()
    {
        return viewModel.MainDefense1!.HasCards;
    }
    private async Task MainDefense1_BoardClickedAsync()
    {
        if (gameContainer!.OtherTurn == 0)
        {
            throw new CustomBasicException("Should not have allowed the click for main defense because not the other turn to defend themselves");
        }
        if (mainGame!.SingleInfo!.MainHandList.HasSelectedObject() == false && mainGame.SingleInfo.DefenseList.HasSelectedObject() == false)
        {
            Toast.ShowUserErrorToast("Must choose at least a card from defense or at least a card from hand");
            return;
        }
        if (mainGame.SingleInfo.MainHandList.HasSelectedObject() && mainGame.SingleInfo.DefenseList.HasSelectedObject())
        {
            Toast.ShowUserErrorToast("Cannot choose cards from both from hand and from defense piles");
            return;
        }
        bool fromHand;
        DeckRegularDict<RegularSimpleCard> tempDefenseList;
        if (mainGame.SingleInfo.MainHandList.HasSelectedObject())
        {
            fromHand = true;
            tempDefenseList = mainGame.SingleInfo.MainHandList.GetSelectedItems();
        }
        else
        {
            fromHand = false;
            tempDefenseList = mainGame.SingleInfo.DefenseList.GetSelectedItems();
            if (tempDefenseList.Any(items => items.Color == EnumRegularColorList.Red))
            {
                throw new CustomBasicException("No red attack cards should have even been put to defense pile");
            }
        }
        if (mainGame.CanPlayDefenseCards(tempDefenseList) == false)
        {
            return;
        }
        if (viewModel.MainDefense1!.CanAddDefenseCards(tempDefenseList) == false)
        {
            Toast.ShowUserErrorToast("This defense is not enough to prevent a goal from the attacker");
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            var deckList = tempDefenseList.GetDeckListFromObjectList();
            if (fromHand == true)
            {
                await mainGame.Network!.SendAllAsync("defensehand", deckList);
            }
            else
            {
                await mainGame.Network!.SendAllAsync("defenseboard", deckList);
            }
        }
        tempDefenseList.UnselectAllObjects();
        await mainGame.PlayDefenseCardsAsync(tempDefenseList, fromHand);
    }
    private Task YourDefensePile_ObjectClickedAsync(RegularSimpleCard thisObject, int index)
    {
        if (mainGame!.SingleInfo!.MainHandList.HasSelectedObject() == true)
        {
            return Task.CompletedTask;
        }
        thisObject.IsSelected = !thisObject.IsSelected;
        return Task.CompletedTask;
    }
    private async Task YourDefensePile_BoardClickedAsync()
    {
        if (gameContainer!.OtherTurn > 0)
        {
            return;
        }
        if (mainGame!.SingleInfo!.MainHandList.HasSelectedObject() == false)
        {
            return;
        }
        if (mainGame.SingleInfo.DefenseList.HasSelectedObject() == true)
        {
            Toast.ShowUserErrorToast("Cannot choose any defense cards on the board to add more defense cards");
            return;
        }
        var tempDefenseList = mainGame.SingleInfo.MainHandList.GetSelectedItems();
        if (mainGame.CanPlayDefenseCards(tempDefenseList) == false)
        {
            return;
        }
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("addtodefense", tempDefenseList.GetDeckListFromObjectList());
        }
        tempDefenseList.UnselectAllObjects();
        await mainGame.AddCardsToDefensePileAsync(tempDefenseList);
    }
    private async Task YourAttackPile_BoardClickedAsync()
    {
        var tempAttackList = mainGame!.SingleInfo!.MainHandList.GetSelectedItems();
        if (mainGame.CanPlayAttackCards(tempAttackList) == false)
        {
            return;
        }
        tempAttackList.UnselectAllObjects();
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("attack", tempAttackList.GetDeckListFromObjectList());
        }
        await mainGame.PlayAttackCardsAsync(tempAttackList);
    }
    public bool CanPass => mainGame!.SaveRoot!.PlayOrder.OtherTurn > 0;
    [Command(EnumCommandCategory.Game)]
    public async Task PassAsync()
    {
        if (mainGame.BasicData!.MultiPlayer == true)
        {
            await mainGame.Network!.SendAllAsync("passdefense");
        }
        await mainGame!.PassDefenseAsync();
    }
}