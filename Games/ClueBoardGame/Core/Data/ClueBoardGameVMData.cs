namespace ClueBoardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[UseStateChanged]
public partial class ClueBoardGameVMData : IDiceBoardGamesData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [StateProperty]
    public partial int LeftToMove { get; set; }
    [LabelColumn]
    public partial string CurrentRoomName { get; set; } = "";

    partial void OnCurrentRoomNameChanged(string value)
    {
        StateChanged?.Invoke();
    }

    [LabelColumn]
    public partial string CurrentCharacterName { get; set; } = "";
    partial void OnCurrentCharacterNameChanged(string value)
    {
        StateChanged?.Invoke();
    }
    [LabelColumn]
    public partial string CurrentWeaponName { get; set; } = "";
    partial void OnCurrentWeaponNameChanged(string value)
    {
        StateChanged?.Invoke();
    }
    public DiceCup<SimpleDice>? Cup { get; set; }
    public string Instructions { get; set; } = ""; //has to be here in order to implement the interface for idiceboarddata

    private readonly CommandContainer _command;
    private readonly IGamePackageResolver _resolver;
    public HandObservable<CardInfo> HandList;
    public SingleObservablePile<CardInfo> Pile;
    public ClueBoardGameVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        _command = command;
        _resolver = resolver;
        HandList = new(command)
        {
            AutoSelect = EnumHandAutoType.None,
            Maximum = 3,
            Text = "Your Cards"
        };
        Pile = new(command);
        Pile.CurrentOnly = true;
        Pile.Text = "Clue";
    }
    public void LoadCup(ISavedDiceList<SimpleDice> saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, _resolver, _command);
        if (autoResume == true)
        {
            Cup.CanShowDice = true;
        }
        Cup.HowManyDice = 1; //you specify how many dice here.
        Cup.Visible = true; //i think.

    }
}