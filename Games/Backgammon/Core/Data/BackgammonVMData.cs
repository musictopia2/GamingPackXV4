namespace Backgammon.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class BackgammonVMData(IGamePackageResolver resolver, CommandContainer command) : IDiceBoardGamesData
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial string Instructions { get; set; } = "";
    [LabelColumn]
    public partial int MovesMade { get; set; }
    [LabelColumn]
    public partial string LastStatus { get; set; } = "";
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
        Cup.HowManyDice = 2;
        Cup.Visible = true;
    }
}