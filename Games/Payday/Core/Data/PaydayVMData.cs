namespace Payday.Core.Data;
[SingletonGame]
[UseLabelGrid]
public partial class PaydayVMData : IDiceBoardGamesData
{
    public string PopUpChosen { get; set; } = "";
    private readonly CommandContainer _command;
    private readonly IGamePackageResolver _resolver;
    public PaydayVMData(CommandContainer command, IGamePackageResolver resolver)
    {
        _command = command;
        _resolver = resolver;
        PopUpList = new(command, resolver);
        PopUpList.IndexMethod = ListViewPicker.EnumIndexMethod.OneBased;
        PopUpList.ItemSelectedAsync += PopUpList_ItemSelectedAsync;
        CurrentDealList = new(command);
        CurrentDealList.AutoSelect = EnumHandAutoType.None;
        CurrentDealList.Text = "Deal List";
        CurrentMailList = new(command);
        CurrentMailList.Text = "Mail List";
        DealPile = new(command);
        DealPile.CurrentOnly = true;
        DealPile.Text = "Deal Pile";
        MailPile = new(command);
        MailPile.CurrentOnly = true;
        MailPile.Text = "Mail Pile";
    }
    private Task PopUpList_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        PopUpChosen = selectedText;
        return Task.CompletedTask;
    }
    public SingleObservablePile<DealCard> DealPile;
    public SingleObservablePile<MailCard> MailPile;
    public HandObservable<DealCard> CurrentDealList;
    public HandObservable<MailCard> CurrentMailList;
    public ListViewPicker PopUpList;
    internal void AddPopupLists(BasicList<string> list)
    {
        PopUpChosen = "";
        PopUpList.LoadTextList(list);
        PopUpList.UnselectAll();
        if (list.Count == 1)
        {
            PopUpChosen = list.Single();
            PopUpList.SelectSpecificItem(1);
        }
    }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
    [LabelColumn] //just in case.
    public EnumStatus GameStatus { get; set; }
    [LabelColumn]
    public string MonthLabel { get; set; } = "";
    [LabelColumn]
    public string OtherLabel { get; set; } = "";
    public DiceCup<SimpleDice>? Cup { get; set; }
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
        Cup.HowManyDice = 1;
        Cup.Visible = true;
    }
}