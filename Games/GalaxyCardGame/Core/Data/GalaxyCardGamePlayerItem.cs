namespace GalaxyCardGame.Core.Data;
[UseScoreboard]
public partial class GalaxyCardGamePlayerItem : PlayerTrick<EnumSuitList, GalaxyCardGameCardInformation>
{
    [JsonIgnore]
    public HandObservable<GalaxyCardGameCardInformation>? PlanetHand { get; set; }
    public string PlanetData { get; set; } = "";
    [JsonIgnore]
    public MainSetsObservable<EnumSuitList, EnumRegularColorList, GalaxyCardGameCardInformation, MoonClass, SavedSet>? Moons { get; set; }
    public BasicList<SavedSet> SavedMoonList { get; set; } = new();
    private GalaxyCardGameMainGameClass? _mainGame;
    private GalaxyCardGameVMData? _model;
    private IToast? _toast;
    public void LoadPiles(GalaxyCardGameMainGameClass mainGame, GalaxyCardGameVMData model, CommandContainer command, IToast toast)
    {
        _mainGame = mainGame;
        _model = model;
        _toast = toast;
        PlanetHand = new(command);
        PlanetHand.BoardClickedAsync += PlanetHand_BoardClickedAsync;
        PlanetHand.AutoSelect = EnumHandAutoType.ShowObjectOnly; //try this way.
        PlanetHand.Maximum = 2;
        PlanetHand.Visible = true;
        PlanetHand.Text = $"{NickName} Planet";
        Moons = new(command);
        Moons.SetClickedAsync += Moons_SetClickedAsync;
        Moons.HasFrame = true;
        Moons.Text = $"{NickName} Moons";
    }
    public void ReportChange()
    {
        if (Moons == null || PlanetHand == null)
        {
            return;
        }
        Moons.ReportCanExecuteChange();
        PlanetHand.ReportCanExecuteChange();
    }
    public bool CanEnableMoon()
    {
        if (_mainGame!.SaveRoot!.GameStatus != EnumGameStatus.PlaceSets)
        {
            return false;
        }
        if (_mainGame.HasAutomaticPlanet())
        {
            return false;
        }
        return PlanetHand!.HandList.Count > 0;
    }
    private async Task Moons_SetClickedAsync(int setNumber, int section, int deck)
    {
        MoonClass thisMoon = Moons!.GetIndividualSet(setNumber);
        if (_mainGame!.CanAddToMoon(thisMoon, out GalaxyCardGameCardInformation? thisCard, out string message) == false)
        {
            _toast!.ShowUserErrorToast($"{message} {Constants.VBCrLf}{Constants.VBCrLf}Because of the reason above, was unable to expand the moon");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            SendExpandedMoon temps = new();
            temps.MoonID = setNumber;
            temps.Deck = thisCard!.Deck;
            await _mainGame.Network!.SendAllAsync("expandmoon", temps);
        }
        await _mainGame.AddToMoonAsync(thisCard!.Deck, setNumber);
    }
    private async Task PlanetHand_BoardClickedAsync()
    {
        var thisList = _model!.PlayerHand1!.ListSelectedObjects();
        if (thisList.Count > 2)
        {
            _toast!.ShowUserErrorToast("Can only select 2 cards at the most for the planet");
            return;
        }
        if (_mainGame!.HasValidPlanet(thisList) == false)
        {
            _toast!.ShowUserErrorToast("This is not a valid planet");
            return;
        }
        if (_mainGame.BasicData!.MultiPlayer)
        {
            var tempList = thisList.GetDeckListFromObjectList();
            await _mainGame.Network!.SendAllAsync("createplanet", tempList);
        }
        await _mainGame.CreatePlanetAsync(thisList);
    }
}