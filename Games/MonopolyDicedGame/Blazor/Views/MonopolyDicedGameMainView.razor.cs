namespace MonopolyDicedGame.Blazor.Views;
public partial class MonopolyDicedGameMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
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
    private bool _rolling;
    private async Task TestRollRegularAsync()
    {
        if (_rolling)
        {
            return;
        }
        _rolling = true;
        var list = _monopolySets!.RollDice();
        await _monopolySets.ShowRollingAsync(list);
        _rolling = false;
    }
    private void SampleClickUtility(EnumUtilityType utility)
    {
        OwnedModel own = new();
        own.UsedOn = EnumBasicType.Utility;
        own.Utility = utility;
        _container!.SaveRoot.Owns.Add(own);
    }
    private void SampleClickTrain()
    {
        OwnedModel own = new();
        own.UsedOn = EnumBasicType.Railroad;
        _container!.SaveRoot.Owns.Add(own);
    }
    private void SamplePropertyClicked(int group)
    {
        OwnedModel own = new();
        own.Group = group;
        _container!.SaveRoot.Owns.Add(own);
    }
    //private async Task TestPropertyClicked(int group)
    //{
    //    await Message!.ShowMessageAsync($"Clicked on {group}");
    //}

    //keep here so i have an idea of what is eventually needed.
    public void SamplePlacement()
    {
        //this will test the placement.

        _container!.SaveRoot.Owns.Clear();

        //will pretend like you are placing 5 items.
        OwnedModel own = new();
        own.UsedOn = EnumBasicType.Railroad;
        own.WasChance = true;
        _container.SaveRoot.Owns.Add(own);
        own = new();
        own.UsedOn = EnumBasicType.Railroad;
        _container.SaveRoot.Owns.Add(own);

        



        own = new();
        own.Utility = EnumUtilityType.Water;
        own.WasChance = true;
        own.UsedOn = EnumBasicType.Utility;
        _container.SaveRoot.Owns.Add(own);
        own = new();
        own.Utility = EnumUtilityType.Electric;
        own.UsedOn = EnumBasicType.Utility;
        _container.SaveRoot.Owns.Add(own);
        own = new();
        own.Group = 8;
        _container.SaveRoot.Owns.Add(own);
        own = new();
        own.Group = 2;
        _container.SaveRoot.Owns.Add(own);
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
        //hopefully registers with vmdata (?)
        _container.SaveRoot.NumberOfCops += _others.Count(x => x == EnumMiscType.Police);
        if (_others.Any(x => x == EnumMiscType.Free))
        {
            _container.SaveRoot.NumberOfCops--;
        }
        if (_others.Any(x => x == EnumMiscType.Go))
        {
            _container.SaveRoot.CurrentScore += 200;
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