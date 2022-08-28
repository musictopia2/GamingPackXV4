namespace Countdown.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class CountdownVMData : IBasicDiceGamesData<CountdownDice>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    [LabelColumn]
    public int RollNumber { get; set; }
    [LabelColumn]
    public int Round { get; set; }
    private readonly IGamePackageResolver _resolver;
    private readonly CommandContainer _command;
    public DiceCup<CountdownDice>? Cup { get; set; }
    public CountdownVMData(IGamePackageResolver resolver, CommandContainer command)
    {
        _resolver = resolver;
        _command = command;
    }
    public void LoadCup(ISavedDiceList<CountdownDice> saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<CountdownDice>(saveRoot.DiceList, _resolver, _command);
        if (autoResume == true)
        {
            Cup.CanShowDice = true;
        }
        Cup.HowManyDice = 2;
        Cup.Visible = true; //i think.
    }
    public static bool ShowHints { get; set; }
    public static Func<SimpleNumber, bool>? CanChooseNumber { get; set; }
}