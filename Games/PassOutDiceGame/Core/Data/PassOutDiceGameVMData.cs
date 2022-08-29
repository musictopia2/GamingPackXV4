namespace PassOutDiceGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class PassOutDiceGameVMData : IDiceBoardGamesData
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    private readonly IGamePackageResolver _resolver;
    private readonly CommandContainer _command;
    public DiceCup<SimpleDice>? Cup { get; set; }
    public PassOutDiceGameVMData(IGamePackageResolver resolver, CommandContainer command)
    {
        _resolver = resolver;
        _command = command;
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
        Cup.HowManyDice = 2; //you specify how many dice here.
        Cup.Visible = true; //i think.

    }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}