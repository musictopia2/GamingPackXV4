namespace SnagCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class SnagCardGameVMData : ITrickCardGamesData<SnagCardGameCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    [LabelColumn]
    public string Instructions { get; set; } = "";
    public SnagCardGameVMData(CommandContainer command,
           SnagTrickObservable trickArea1,
           SnagCardGameGameContainer gameContainer
           )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        TrickArea1 = trickArea1;
        Bar1 = new BarObservable(gameContainer);
        Human1 = new HandObservable<SnagCardGameCardInformation>(command);
        Opponent1 = new HandObservable<SnagCardGameCardInformation>(command);
        Bar1.Visible = true;
        Bar1.AutoSelect = EnumHandAutoType.SelectOneOnly;
        Human1.Text = "Your Cards Won";
        Opponent1.Text = "Opponent Cards Won";
        Human1.Visible = false;
        Opponent1.Visible = false;
    }
    BasicTrickAreaObservable<EnumSuitList, SnagCardGameCardInformation> ITrickCardGamesData<SnagCardGameCardInformation, EnumSuitList>.TrickArea1
    {
        get => TrickArea1;
        set => TrickArea1 = (SnagTrickObservable)value;
    }
    public BarObservable Bar1;
    public HandObservable<SnagCardGameCardInformation> Human1;
    public HandObservable<SnagCardGameCardInformation> Opponent1;
    public SnagTrickObservable TrickArea1 { get; set; }
    public DeckObservablePile<SnagCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<SnagCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<SnagCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<SnagCardGameCardInformation>? OtherPile { get; set; }
}