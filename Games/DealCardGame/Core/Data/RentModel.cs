namespace DealCardGame.Core.Data;
public class RentModel
{
    public int Deck { get; set; }
    //refer to payday and game of life (that has a player picker).
    //if only 2 total players, then not needed.
    public int Player { get; set; } //only if allowed.
    public EnumColor Color { get; set; }
    public EnumRentCategory RentCategory { get; set; } = EnumRentCategory.NA; //start out with not even applicable.
}