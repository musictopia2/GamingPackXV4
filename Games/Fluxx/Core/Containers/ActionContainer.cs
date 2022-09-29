namespace Fluxx.Core.Containers;
[SingletonGame]
[AutoReset]
public class ActionContainer : IEnableAlways, IBasicEnableProcess
{
    private readonly CommandContainer _command;
    private readonly FluxxGameContainer _gameContainer;
    public ActionContainer(CommandContainer command, IGamePackageResolver resolver, FluxxGameContainer gameContainer, FluxxDelegates delegates)
    {
        delegates.RefreshEnables = RefreshEnables;
        _command = command;
        _gameContainer = gameContainer;
        ActionDetail = new ();
        CurrentDetail = new ();
        YourKeepers = new(command)
        {
            AutoSelect = EnumHandAutoType.ShowObjectOnly,
            Text = "Your Keepers"
        };
        YourKeepers.SendAlwaysEnable(this);
        YourKeepers.ConsiderSelectOneAsync = YourKeepers_ConsiderSelectOneAsync;
        PrivateGoals = new(command)
        {
            AutoSelect = EnumHandAutoType.ShowObjectOnly,
            Text = "Goal Cards",
            Maximum = 3
        };
        PrivateGoals.ConsiderSelectOneAsync = PrivateGoals_ConsiderSelectOneAsync;
        YourCards = new(command)
        {
            Text = "Your Cards"
        };
        YourCards.ConsiderSelectOneAsync = YourCards_ConsiderSelectOneAsync;
        YourCards.AutoSelect = EnumHandAutoType.ShowObjectOnly;
        OtherHand = new(command)
        {
            AutoSelect = EnumHandAutoType.SelectOneOnly,
            Text = "Other Player's Cards"
        };
        OtherHand.ConsiderSelectOneAsync = YourCards_ConsiderSelectOneAsync;
        OtherHand.SendEnableProcesses(this, () =>
        {
            if (OtherHand.Visible == false)
            {
                return false;
            }
            return ActionCategory == EnumActionCategory.FirstRandom || ActionCategory == EnumActionCategory.UseTake;
        });
        TempHand = new(command)
        {
            AutoSelect = EnumHandAutoType.SelectOneOnly,
            Text = "Temporary Cards"
        };
        TempHand.ConsiderSelectOneAsync = YourCards_ConsiderSelectOneAsync;
        TempHand.SendEnableProcesses(this, () =>
        {
            return ActionCategory == EnumActionCategory.Everybody1 || ActionCategory == EnumActionCategory.DrawUse;
        });
        Direction1 = new ListViewPicker(command, resolver);
        Direction1.ItemSelectedAsync = Direction1_ItemSelectedAsync;
        Direction1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
        Direction1.LoadTextList(new() { "Left", "Right" });
        Rule1 = new ListViewPicker(command, resolver);
        Rule1.ItemSelectedAsync = Rule1_ItemSelectedAsync;
        Rule1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
        Rule1.SendEnableProcesses(this, () =>
        {
            return ActionCategory == EnumActionCategory.Rules;
        });
        Player1 = new ListViewPicker(command, resolver);
        Player1.ItemSelectedAsync = Player1_ItemSelectedAsync;
        Player1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
        Player1.SendEnableProcesses(this, () => CanEnableChoosePlayer());

        CardList1 = new ListViewPicker(command, resolver);
        CardList1.ItemSelectedAsync = CardList1_ItemSelectedAsync;
        CardList1.IndexMethod = ListViewPicker.EnumIndexMethod.ZeroBased;
        CardList1.SendEnableProcesses(this, () =>
        {
            return ActionCategory == EnumActionCategory.DoAgain;
        });
    }
    internal BasicList<int> PlayerIndexList { get; set; } = new();
    internal int Loads { get; set; }
    public DetailCardObservable ActionDetail;
    public DetailCardObservable CurrentDetail;
    public ListViewPicker Direction1;
    public ListViewPicker Rule1;
    public ListViewPicker Player1;
    public ListViewPicker CardList1;
    public BasicList<int>? TempRuleList = new();
    public HandObservable<GoalCard> PrivateGoals;
    public HandObservable<FluxxCardInformation> YourCards;
    public HandObservable<KeeperCard> YourKeepers;
    public HandObservable<FluxxCardInformation> OtherHand;
    public HandObservable<FluxxCardInformation> TempHand;
    public int GetPlayerIndex(int selectedIndex)
    {
        return PlayerIndexList[selectedIndex];
    }
    private Task CardList1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        IndexCard = selectedIndex;
        return Task.CompletedTask;
    }
    private Task Player1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        IndexPlayer = selectedIndex;
        return Task.CompletedTask;
    }
    private Task YourCards_ConsiderSelectOneAsync(FluxxCardInformation thisObject)
    {
        ShowCard(thisObject);
        return Task.CompletedTask;
    }
    private Task PrivateGoals_ConsiderSelectOneAsync(GoalCard thisObject)
    {
        ShowCard(thisObject);
        return Task.CompletedTask;
    }
    private Task YourKeepers_ConsiderSelectOneAsync(KeeperCard thisObject)
    {
        ShowCard(thisObject);
        return Task.CompletedTask;
    }
    private Task Direction1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        IndexDirection = selectedIndex;
        return Task.CompletedTask;
    }
    private Task Rule1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        if (_gameContainer.CurrentAction!.Deck == EnumActionMain.TrashANewRule)
        {
            IndexRule = selectedIndex;
        }
        else
        {
            TempRuleList = Rule1!.GetAllSelectedItems();
        }
        return Task.CompletedTask;
    }

    public void ShowCard(FluxxCardInformation card)
    {
        if (card.IsUnknown == true)
        {
            return;
        }
        if (card.Deck == CurrentDetail!.CurrentCard.Deck)
        {
            CurrentDetail.ResetCard();
        }
        else
        {
            CurrentDetail.ShowCard(card);
        }
    }
    public EnumActionCategory ActionCategory { get; set; }
    public int RulesToDiscard { get; set; }
    #region "Filled Properties
    public int IndexPlayer { get; set; } = -1;
    public int IndexDirection { get; set; } = -1;
    public int IndexRule { get; set; } = -1;
    public int IndexCard { get; set; }
    public string ActionFrameText { get; set; } = "Action Card Information";
    public bool ButtonChoosePlayerVisible { get; set; }
    public bool ButtonChooseCardVisible { get; set; }
    #endregion
    bool IEnableAlways.CanEnableAlways()
    {
        return true;
    }
    bool IBasicEnableProcess.CanEnableBasics()
    {
        return true;
    }
    public void SetUpGoals()
    {
        _gameContainer!.SaveRoot!.SavedActionData.SelectedIndex = -1;
        IndexPlayer = -1;
        Player1!.UnselectAll();
    }
    public void SetUpFrames()
    {
        if (_gameContainer.SingleInfo!.PlayerCategory != EnumPlayerCategory.Self)
        {
            throw new CustomBasicException("Needs to be set to self first");
        }
        YourCards!.HandList = _gameContainer.SingleInfo.MainHandList;
        YourKeepers!.HandList = _gameContainer.SingleInfo.KeeperList;
        PrivateGoals!.HandList = _gameContainer.SaveRoot!.GoalList;
    }
    public FluxxCardInformation GetCardToDoAgain(int selectedIndex)
    {
        var tempList = _gameContainer.DeckList.Where(items => items.CardType == EnumCardType.Rule || items.CardType == EnumCardType.Action).ToRegularDeckDict();
        var thisText = CardList1!.GetText(selectedIndex);
        return tempList.Single(items => items.Text() == thisText);
    }
    private bool CanEnableChoosePlayer()
    {
        if (ActionCategory == EnumActionCategory.TradeHands || ActionCategory == EnumActionCategory.UseTake || ActionCategory == EnumActionCategory.Everybody1)
        {
            if (ActionCategory == EnumActionCategory.UseTake && ButtonChoosePlayerVisible == false)
            {
                return false;
            }
            if (_gameContainer.CurrentAction == null)
            {
                return false;
            }
            var thisC = _gameContainer.CurrentAction!.Deck;
            if (thisC == EnumActionMain.UseWhatYouTake && _gameContainer.SaveRoot!.SavedActionData.SelectedIndex == -1 || thisC != EnumActionMain.UseWhatYouTake)
            {
                return true;
            }
        }
        return false;
    }
    public async Task DoAgainProcessPart1Async(int selectedIndex)
    {
        _gameContainer.SingleInfo = _gameContainer.PlayerList!.GetWhoPlayer();
        if (_gameContainer.SingleInfo.CanSendMessage(_gameContainer.BasicData!))
        {
            await _gameContainer.Network!.SendAllAsync("doagain", selectedIndex);
        }
    }
    public async Task ChosePlayerOnActionAsync(int selectedIndex)
    {
        if (_gameContainer.SingleInfo!.CanSendMessage(_gameContainer.BasicData!))
        {
            await _gameContainer.Network!.SendAllAsync("choseplayerforcardchosen", selectedIndex);
        }
    }
    public bool CanChoosePlayer()
    {
        if (ButtonChoosePlayerVisible == false || IndexPlayer == -1)
        {
            return false;
        }
        return CanEnableChoosePlayer();
    }
    public void RefreshEnables()
    {
        _command.ManuelFinish = false;
    }
}
