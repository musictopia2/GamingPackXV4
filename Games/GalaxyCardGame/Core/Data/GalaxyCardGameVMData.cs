namespace GalaxyCardGame.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class GalaxyCardGameVMData : ITrickCardGamesData<GalaxyCardGameCardInformation, EnumSuitList>
{
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public EnumSuitList TrumpSuit { get; set; }
    public GalaxyCardGameVMData(CommandContainer command,
            BasicTrickAreaObservable<EnumSuitList, GalaxyCardGameCardInformation> trickArea1
            )
    {
        Deck1 = new(command);
        Pile1 = new(command);
        PlayerHand1 = new(command);
        PlayerHand1.Maximum = 9;
        TrickArea1 = trickArea1;
    }
    public BasicTrickAreaObservable<EnumSuitList, GalaxyCardGameCardInformation> TrickArea1 { get; set; }
    public DeckObservablePile<GalaxyCardGameCardInformation> Deck1 { get; set; }
    public SingleObservablePile<GalaxyCardGameCardInformation> Pile1 { get; set; }
    public HandObservable<GalaxyCardGameCardInformation> PlayerHand1 { get; set; }
    public SingleObservablePile<GalaxyCardGameCardInformation>? OtherPile { get; set; }
}