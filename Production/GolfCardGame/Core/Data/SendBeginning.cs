namespace GolfCardGame.Core.Data;
public class SendBeginning
{
    public int Player { get; set; }
    public DeckRegularDict<RegularSimpleCard> SelectList { get; set; } = [];
    public DeckRegularDict<RegularSimpleCard> UnsSelectList { get; set; } = [];
}