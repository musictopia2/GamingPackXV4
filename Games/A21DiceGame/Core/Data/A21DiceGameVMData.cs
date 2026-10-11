namespace A21DiceGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class A21DiceGameVMData(IGamePackageResolver resolver, CommandContainer command) : IBasicDiceGamesData<SimpleDice>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    [LabelColumn]
    public partial int RollNumber { get; set; }
    public DiceCup<SimpleDice>? Cup { get; set; }

    public void LoadCup(ISavedDiceList<SimpleDice> saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<SimpleDice>(saveRoot.DiceList, resolver, command);
        if (autoResume == true)
        {
            Cup.CanShowDice = true;
        }
        Cup.HowManyDice = 1; //you specify how many dice here.
        Cup.Visible = true; //i think.

    }
    //any other ui related properties will be here.
    //can copy/paste for the actual view model.
}