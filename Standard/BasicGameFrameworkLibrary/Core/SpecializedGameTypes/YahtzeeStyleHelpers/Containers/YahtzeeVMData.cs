namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
[UseLabelGrid]
public partial class YahtzeeVMData<D>(CommandContainer command, IGamePackageResolver resolver) : IBasicDiceGamesData<D>
    where D : SimpleDice, new()
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int RollNumber { get; set; }
    [LabelColumn]
    public partial int Round { get; set; }
    [LabelColumn]
    public partial int Score { get; set; }
    [LabelColumn]
    public partial string CategoryChosen { get; set; } = "None";
    public DiceCup<D>? Cup { get; set; }
    public void LoadCup(ISavedDiceList<D> saveRoot, bool autoResume)
    {
        if (Cup != null && autoResume)
        {
            return;
        }
        Cup = new DiceCup<D>(saveRoot.DiceList, resolver, command);
        if (autoResume == true)
        {
            Cup.CanShowDice = true;
        }
        Cup.HowManyDice = 5;
        Cup.Visible = true;
        Cup.ShowHold = true;
    }
}