namespace Millebournes.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class MillebournesVMData : IBasicCardGamesData<MillebournesCardInformation>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    public bool CoupeVisible { get; set; }
    public MillebournesVMData(CommandContainer command)
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        OtherPile = Pile1;
        Pile2 = new(command);

        Stops = new();
        Stops.MaxTime = 3000;
    }
    public DeckObservablePile<MillebournesCardInformation> Deck1 { get; set; }
    public SingleObservablePile<MillebournesCardInformation> Pile1 { get; set; }
    public SingleObservablePile<MillebournesCardInformation> Pile2 { get; set; }
    public HandObservable<MillebournesCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<MillebournesCardInformation>? OtherPile { get; set; }
    public CustomStopWatchCP Stops;
}