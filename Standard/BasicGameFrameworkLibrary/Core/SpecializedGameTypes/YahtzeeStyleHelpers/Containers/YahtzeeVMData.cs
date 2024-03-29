﻿namespace BasicGameFrameworkLibrary.Core.SpecializedGameTypes.YahtzeeStyleHelpers.Containers;
[UseLabelGrid]
public sealed partial class YahtzeeVMData<D>(CommandContainer command, IGamePackageResolver resolver) : IBasicDiceGamesData<D>
    where D : SimpleDice, new()
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int RollNumber { get; set; }
    [LabelColumn]
    public int Round { get; set; }
    [LabelColumn]
    public int Score { get; set; }
    [LabelColumn]
    public string CategoryChosen { get; set; } = "None";
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