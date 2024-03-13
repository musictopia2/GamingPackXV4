namespace DealCardGame.Core.Data;
[SingletonGame]
public class DealCardGameSaveInfo : BasicSavedCardClass<DealCardGamePlayerItem, DealCardGameCardInformation>, IMappable, ISaveInfo
{
    public EnumGameStatus GameStatus { get; set; } = EnumGameStatus.None;
    public int ActionCardUsed { get; set; } //this is the action card that was used.

    //may know which player because otherturn would have to be used since they have the choice to just say no.
    public int PlayerUsedAgainst { get; set; } //if the otherplayer just says no, then needs to know who this player is.
    public EnumRentCategory RentCategory { get; set; } //so if you chose to make players pay rent, it knows the category
    public int PaymentOwed { get; set; } //needs to know the payment that is owed (whether its rent, etc).
    public EnumColor OpponentColorChosen { get; set; } = EnumColor.None;
    public int CardStolen { get; set; }
    public EnumColor YourColorChosen { get; set; } = EnumColor.None;
    public int OpponentTrade { get; set; } //this is the opponent card you want
    public int YourTrade { get; set; } //this is the card you are giving up.
}