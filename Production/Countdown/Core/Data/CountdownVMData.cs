namespace Countdown.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class CountdownVMData(IGamePackageResolver resolver, CommandContainer command) : IBasicDiceGamesData<CountdownDice>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    [LabelColumn]
    public partial int RollNumber { get; set; }
    [LabelColumn]
    public partial int Round { get; set; }
    public DiceCup<CountdownDice>? Cup { get; set; }

    public void LoadCup(ISavedDiceList<CountdownDice> saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<CountdownDice>(saveRoot.DiceList, resolver, command);
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