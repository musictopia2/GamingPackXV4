namespace Xactika.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class XactikaVMData : ITrickCardGamesData<XactikaCardInformation, EnumShapes>, IBasicEnableProcess
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumShapes TrumpSuit { get; set; }
    [LabelColumn]
    public string GameModeText { get; set; } = "";
    [LabelColumn]
    public EnumShapes ShapeChosen { get; set; }
    [LabelColumn]
    public int RoundNumber { get; set; }
    [LabelColumn]
    public EnumGameMode ModeChosen { get; set; }
    private readonly XactikaGameContainer _gameContainer;
    public XactikaVMData(CommandContainer command,
        BasicTrickAreaObservable<EnumShapes, XactikaCardInformation> trickArea1,
        XactikaGameContainer gameContainer
        )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        _gameContainer = gameContainer;
        ModeChoose1 = new ListViewPicker(command, gameContainer.Resolver);
        ShapeChoose1 = new ChooseShapeObservable(_gameContainer);
        Bid1 = new NumberPicker(command, gameContainer.Resolver);
        ModeChoose1.ItemSelectedAsync = ModeChooser1_ItemSelectedAsync;
        Bid1.ChangedNumberValueAsync = Bid1_ChangedNumberValueAsync;
        PlayerHand1!.Maximum = 8;
        ModeChoose1.IndexMethod = EnumIndexMethod.OneBased;
        BasicList<string> tempList = new() { "To Win", "To Lose", "Bid" };
        ModeChoose1.LoadTextList(tempList);
        ShapeChoose1.SendEnableProcesses(this, () => _gameContainer.SaveRoot.GameStatus == EnumStatusList.CallShape);
    }
    private Task ModeChooser1_ItemSelectedAsync(int selectedIndex, string selectedText)
    {
        _gameContainer!.SaveRoot!.GameMode = (EnumGameMode)selectedIndex;
        return Task.CompletedTask;
    }
    private Task Bid1_ChangedNumberValueAsync(int chosen)
    {
        BidChosen = chosen;
        return Task.CompletedTask;
    }
    bool IBasicEnableProcess.CanEnableBasics()
    {
        return true;
    }

    public ChooseShapeObservable ShapeChoose1;
    public NumberPicker Bid1;
    [LabelColumn]
    public int BidChosen { get; set; } = -1;
    public ListViewPicker ModeChoose1;
    public BasicTrickAreaObservable<EnumShapes, XactikaCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<XactikaCardInformation> Deck1 { get; set; }
    public SingleObservablePile<XactikaCardInformation> Pile1 { get; set; }
    public HandObservable<XactikaCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<XactikaCardInformation>? OtherPile { get; set; }
}