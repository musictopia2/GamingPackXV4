namespace Cribbage.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class CribbageVMData : IBasicCardGamesData<CribbageCard>, IBasicEnableProcess
{
    public CribbageVMData(CommandContainer command, HiddenBoard board)
    {
        Deck1 = new DeckObservablePile<CribbageCard>(command);
        Pile1 = new SingleObservablePile<CribbageCard>(command);
        PlayerHand1 = new HandObservable<CribbageCard>(command);
        MainFrame = new HandObservable<CribbageCard>(command);
        CribFrame = new HandObservable<CribbageCard>(command);
        CribFrame.Visible = false;
        MainFrame.Text = "Card List";
        CribFrame.Text = "Crib";
        MainFrame.SendEnableProcesses(this, () => false);
        CribFrame.SendEnableProcesses(this, () => false);
        GameBoard1 = board;
        ScoreBoard1 = new ScoreBoardCP();

    }
    public ScoreBoardCP ScoreBoard1;
    public HandObservable<CribbageCard> CribFrame;
    public HandObservable<CribbageCard> MainFrame;
    public HiddenBoard GameBoard1;
    public DeckObservablePile<CribbageCard> Deck1 { get; set; }
    public SingleObservablePile<CribbageCard> Pile1 { get; set; }
    public HandObservable<CribbageCard> PlayerHand1 { get; set; }
    public SingleObservablePile<CribbageCard>? OtherPile { get; set; }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public int TotalScore { get; set; }
    [LabelColumn]
    public int TotalCount { get; set; }
    [LabelColumn]
    public string Dealer { get; set; } = "";
    bool IBasicEnableProcess.CanEnableBasics()
    {
        return true;
    }
}