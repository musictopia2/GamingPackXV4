namespace ThinkTwice.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class ThinkTwiceVMData(IGamePackageResolver resolver,
        CommandContainer command,
        ThinkTwiceGameContainer gameContainer
            ) : IBasicDiceGamesData<SimpleDice>
{
    [LabelColumn]
    public partial string NormalTurn { get; set; } = "";
    [LabelColumn]
    public partial string Status { get; set; } = "";
    [LabelColumn]
    public partial int Score { get; set; }
    [LabelColumn]
    public partial int RollNumber { get; set; }
    [LabelColumn]
    public partial string CategoryChosen { get; set; } = "None";
    public string DisplayScoreText { get; set; } = "Category Information";
    public DiceCup<SimpleDice>? Cup { get; set; }
    public BasicList<string> TextList =
        [
            "Different (1, 2, 3, 4, 5, 6)",
            "Even (2, 4, 6)",
            "High (4, 5, 6)",
            "Low (1, 2, 3)",
            "Odd (1, 3, 5)",
            "Same (2, 2, 2)"
        ];

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
        Cup.HowManyDice = 6;
        Cup.Visible = true;
        Cup.ShowHold = true;
    }

    private int _itemSelected;
    public int ItemSelected
    {
        get { return _itemSelected; }
        set
        {
            if (SetProperty(ref _itemSelected, value))
            {
                if (ItemSelected == -1)
                {
                    CategoryChosen = "None";
                }
                else
                {
                    CategoryChosen = TextList[ItemSelected];
                }
                gameContainer.SaveRoot.CategorySelected = value;
            }
        }
    }
    public void ClearBoard()
    {
        ItemSelected = -1;
    }
}