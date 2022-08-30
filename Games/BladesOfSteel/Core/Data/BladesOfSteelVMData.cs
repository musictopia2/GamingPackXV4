
namespace BladesOfSteel.Core.Data;
[SingletonGame]
[UseLabelGrid]
[AutoReset]
public partial class BladesOfSteelVMData : IBasicCardGamesData<RegularSimpleCard>
{
    public BladesOfSteelVMData(CommandContainer command, BladesOfSteelGameContainer gameContainer)
    {
        Deck1 = new DeckObservablePile<RegularSimpleCard>(command);
        Pile1 = new SingleObservablePile<RegularSimpleCard>(command);
        PlayerHand1 = new HandObservable<RegularSimpleCard>(command);
        YourFaceOffCard = new(command);
        YourFaceOffCard.IsEnabled = false;
        YourFaceOffCard.Text = "Your";
        OpponentFaceOffCard = new(command);
        OpponentFaceOffCard.IsEnabled = false;
        OpponentFaceOffCard.Text = "Opponent";
        MainDefense1 = new MainDefenseCP(gameContainer);
        YourAttackPile = new PlayerAttackCP(command);
        YourDefensePile = new PlayerDefenseCP(command);
        OpponentAttackPile = new PlayerAttackCP(command);
        OpponentDefensePile = new PlayerDefenseCP(command);
    }
    public SingleObservablePile<RegularSimpleCard> YourFaceOffCard;
    public SingleObservablePile<RegularSimpleCard> OpponentFaceOffCard;
    public MainDefenseCP MainDefense1;
    public PlayerAttackCP YourAttackPile;
    public PlayerDefenseCP YourDefensePile;
    public PlayerAttackCP OpponentAttackPile;
    public PlayerDefenseCP OpponentDefensePile;
    public DeckObservablePile<RegularSimpleCard> Deck1 { get; set; }
    public SingleObservablePile<RegularSimpleCard> Pile1 { get; set; }
    public HandObservable<RegularSimpleCard> PlayerHand1 { get; set; }
    public SingleObservablePile<RegularSimpleCard>? OtherPile { get; set; }
    [LabelColumn]
    public string NormalTurn { get; set; } = "";
    [LabelColumn]
    public string Status { get; set; } = "";
    [LabelColumn]
    public string OtherPlayer { get; set; } = "";
    [LabelColumn]
    public string Instructions { get; set; } = "";
}