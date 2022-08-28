namespace YachtRace.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class YachtRaceVMData : IBasicDiceGamesData<SimpleDice>
{
    [LabelColumn]
    public string ErrorMessage { get; set; } = ""; //has to be here now.
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    [LabelColumn]
    public int RollNumber { get; set; }
    private readonly IGamePackageResolver _resolver;
    private readonly CommandContainer _command;
    public DiceCup<SimpleDice>? Cup { get; set; }
    public YachtRaceVMData(IGamePackageResolver resolver, CommandContainer command)
    {
        _resolver = resolver;
        _command = command;
        Stops = new Stopwatch();
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
        Cup.HowManyDice = 5; //you specify how many dice here.
        Cup.Visible = true; //i think.
        Cup.ShowHold = true;
    }
    internal Stopwatch Stops { get; set; }
}