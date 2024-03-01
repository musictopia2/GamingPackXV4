namespace MonopolyDicedGame.Blazor.Views;
public partial class MonopolyDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    [Inject]
    private IToast? Toast { get; set; }
    private readonly BasicList<LabelGridModel> _labels = [];
    private BasicList<EnumMiscType> _others = [];
    private MonopolyDicedGameGameContainer? _container;
    private HouseDice? _house;
    private MonopolyDiceSet? _monopolySets;
    protected override void OnInitialized()
    {
        _container = aa1.Resolver!.Resolve<MonopolyDicedGameGameContainer>();
        _house = aa1.Resolver!.Resolve<HouseDice>();
        _monopolySets = aa1.Resolver!.Resolve<MonopolyDiceSet>();
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(MonopolyDicedGameVMData.NormalTurn))
            .AddLabel("Status", nameof(MonopolyDicedGameVMData.Status))
            .AddLabel("Roll", nameof(MonopolyDicedGameVMData.RollNumber))
            .AddLabel("Current Score", nameof(MonopolyDicedGameVMData.CurrentScore))
            ;
        base.OnInitialized();
    }

    private BasicList<BasicDiceModel> GetSelectedDice => _monopolySets!.DiceList.GetSelectedItems();
    private static void OnTestDiceClick(BasicDiceModel dice)
    {
        dice.IsSelected = !dice.IsSelected;
    }
    private void SampleClickUtility(EnumUtilityType utility)
    {
        var list = GetSelectedDice;
        if (list.Count == 0)
        {
            Toast!.ShowUserErrorToast("Did not choose any dice");
            return;
        }
        if (list.Count > 1)
        {
            Toast!.ShowUserErrorToast("Can only choose one item when its utility");
            return;
        }
        int count = _container!.SaveRoot.Owns.Count(x => x.Utility != EnumUtilityType.None);
        if (count == 2)
        {
            Toast!.ShowUserErrorToast("Too many utilities would have been chosen");
            return;
        }
        var dice = list.Single();
        bool hasUsed = _container.SaveRoot.Owns.Any(x => x.Utility == utility);
        if (hasUsed)
        {
            Toast!.ShowUserErrorToast("You already own this utility");
            return;
        }
        if (HasChanceError(list, x => x.UsedOn == EnumBasicType.Utility))
        {
            return;
        }
        if (dice.WhatDice == EnumBasicType.Chance || list.Single().Index == (int)utility)
        {
            dice.IsSelected = false;
            OwnedModel own = new();
            own.UsedOn = EnumBasicType.Utility;
            if (dice.WhatDice == EnumBasicType.Chance)
            {
                own.WasChance = true;
            }
            own.Utility = utility;
            _container!.SaveRoot.Owns.Add(own);
            _monopolySets!.DiceList.RemoveSpecificItem(dice);
            _monopolySets.DiceList.Sort();
            _container.SaveRoot.CurrentScore = _container.SaveRoot.GetTotalScoreInRound();
            return;
        }
        Toast!.ShowUserErrorToast("This is not the proper utility"); //don't unselect automatically.
    }
    private bool HasChanceError(BasicList<BasicDiceModel> list, Func<OwnedModel, bool> selector)
    {
        int chancesSelected = list.Count(x => x.WhatDice == EnumBasicType.Chance);
        if (chancesSelected > 1)
        {
            Toast!.ShowUserErrorToast("You can only choose one chance to place in a group");
            return true;
        }
        int chancesOwned = _container!.SaveRoot.Owns.Count(x => x.WasChance && selector.Invoke(x));
        if (chancesOwned > 0 && chancesSelected > 0)
        {
            Toast!.ShowUserErrorToast("Only one chance can be used per group");
            return true;
        }
        if (chancesOwned + chancesSelected > 1)
        {
            Toast!.ShowUserErrorToast("Only one chance can be used per group");
            return true;
        }
        return false;
    }
    private void SampleClickTrain()
    {
        var list = GetSelectedDice;
        if (list.Count == 0)
        {
            Toast!.ShowUserErrorToast("Did not choose any dice");
            return;
        }
        bool allTrains;
        allTrains = list.All(x =>
        {
            if (x.WhatDice == EnumBasicType.Railroad)
            {
                return true;
            }
            if (x.WhatDice == EnumBasicType.Chance)
            {
                return true;
            }
            return false;
        });
        if (allTrains == false)
        {
            Toast!.ShowUserErrorToast("You have some dice that is not train or chance selected");
            return;
        }
        int owns = _container!.SaveRoot.Owns.Count(x => x.UsedOn == EnumBasicType.Railroad);
        if (owns + list.Count > 4)
        {
            Toast!.ShowUserErrorToast("This results in too many trains.  Only 4 are allowed at the most");
            return;
        }
        if (HasChanceError(list, x => x.UsedOn == EnumBasicType.Railroad))
        {
            return;
        }
        foreach (var item in list)
        {
            OwnedModel own = new();
            own.UsedOn = EnumBasicType.Railroad;
            if (item.WhatDice == EnumBasicType.Chance)
            {
                own.WasChance = true;
            }
            _container!.SaveRoot.Owns.Add(own);
            item.IsSelected = false;
            _monopolySets!.DiceList.RemoveSpecificItem(item);
        }
        _monopolySets!.DiceList.Sort();
        _container.SaveRoot.CurrentScore = _container.SaveRoot.GetTotalScoreInRound();
    }
    private void SamplePropertyClicked(int group)
    {
        bool allProper;
        var list = GetSelectedDice;
        if (list.Count == 0)
        {
            Toast!.ShowUserErrorToast("Did not choose any dice");
            return;
        }
        allProper = list.All(x =>
        {
            if (x.WhatDice == EnumBasicType.Chance)
            {
                return true;
            }
            if (x.Group == group)
            {
                return true;
            }
            return false;
        });
        if (allProper == false)
        {
            Toast!.ShowUserErrorToast("You have dice selected that does not belong to this group");
            return;
        }
        if (HasChanceError(list, x => x.Group == group))
        {
            return;
        }
        int maxAllowed;
        if (group == 1 || group == 8)
        {
            //only 2 are allowed
            maxAllowed = 2;
        }
        else
        {
            maxAllowed = 3;
        }

        int total = list.Count + _container!.SaveRoot.Owns.Count(x => x.Group == group);
        if (total > maxAllowed)
        {
            Toast!.ShowUserErrorToast("This will show too many dice used for this group");
            return;
        }
        foreach (var item in list)
        {
            OwnedModel own = new();
            own.Group = group;
            if (item.WhatDice == EnumBasicType.Chance)
            {
                own.WasChance = true;
            }
            _container!.SaveRoot.Owns.Add(own);
            item.IsSelected = false;
            _monopolySets!.DiceList.RemoveSpecificItem(item);
        }
        _monopolySets!.DiceList.Sort();
        _container.SaveRoot.CurrentScore = _container.SaveRoot.GetTotalScoreInRound();
    }
    private bool _rolling;
    private async Task TestRollAsync()
    {
        if (_rolling)
        {
            return;
        }
        _rolling = true;
        var first = _monopolySets!.RollDice();
        await _monopolySets.ShowRollingAsync(first);
        if (_container!.SaveRoot.HasAtLeastOnePropertyMonopoly)
        {
            var second = _house!.RollDice();
            await _house.ShowRollingAsync(second);
            if (_house.Value == EnumMiscType.Free && _container!.SaveRoot.NumberOfCops > 0)
            {
                _container.SaveRoot.NumberOfCops--;
            }
            if (_house.Value == EnumMiscType.BrokenHouse)
            {
                if (_container!.SaveRoot.NumberOfHouses > 0)
                {
                    _container.SaveRoot.NumberOfHouses--;
                }
            }
            if (_house.Value == EnumMiscType.RegularHouse)
            {
                if (_container!.SaveRoot.NumberOfHouses < 4 && _container.SaveRoot.HasHotel == false)
                {
                    _container.SaveRoot.NumberOfHouses++;
                }
            }
            if (_house.Value == EnumMiscType.Hotel)
            {
                if (_container!.SaveRoot.NumberOfHouses == 4)
                {
                    _container.SaveRoot.HasHotel = true;
                    _container.SaveRoot.NumberOfHouses = 0; //because you now have hotel.
                }
            }
            _container!.SaveRoot.CurrentScore = _container.SaveRoot.GetTotalScoreInRound();
        }
        _rolling = false;
    }

    private bool CanTestRoll() => _container!.SaveRoot.NumberOfCops < 3;
    private void ClearTestRoll()
    {
        _container!.SaveRoot.RollNumber = 1;
        _container.SaveRoot.NumberOfCops = 0;
        _others.Clear();
    }
    private async Task TestRollMiscAsync()
    {

        var list = _house!.RollDice();
        await _house.ShowRollingAsync(list);
        _others = _container!.SaveRoot.GetMiscResults(_container.Random);
        _container.SaveRoot.RollNumber++;
        _container.SaveRoot.NumberOfCops += _others.Count(x => x == EnumMiscType.Police);
        if (_others.Any(x => x == EnumMiscType.Free))
        {
            _container.SaveRoot.NumberOfCops--;
        }
        if (_others.Any(x => x == EnumMiscType.Go))
        {
            _container.SaveRoot.CurrentScore += 200;
            _container.SaveRoot.TotalGos++;
        }
        if (_house.Value == EnumMiscType.Free && _container.SaveRoot.NumberOfCops > 0)
        {
            _container.SaveRoot.NumberOfCops--;
        }
        if (_house.Value == EnumMiscType.BrokenHouse)
        {
            if (_container.SaveRoot.NumberOfHouses > 0)
            {
                _container.SaveRoot.NumberOfHouses--;
            }
        }
        if (_house.Value == EnumMiscType.RegularHouse)
        {
            if (_container.SaveRoot.NumberOfHouses < 4 && _container.SaveRoot.HasHotel == false)
            {
                _container.SaveRoot.NumberOfHouses++;
            }
        }
        if (_house.Value == EnumMiscType.Hotel)
        {
            if (_container.SaveRoot.NumberOfHouses == 4)
            {
                _container.SaveRoot.HasHotel = true;
                _container.SaveRoot.NumberOfHouses = 0; //because you now have hotel.
            }
        }
        if (_container.SaveRoot.NumberOfCops > 2)
        {
            _container.SaveRoot.CurrentScore = 0; //this means 0 points period.
        }
        //for now, the code is here.  will eventually move to somewhere else.
        _container.Command.UpdateAll();
    }
}