namespace MonasteryCardGame.Core.Data;
public class MonasteryCardInfo : RegularRummyCard
{
    public int Temp { get; set; } //this is used so it can still populate properly.
    protected override void FinishPopulatingCard()
    {
        if (Deck == 0)
        {
            throw new CustomBasicException("Deck cannot be 0 when finish populating");
        }
        if (Temp == 0)
        {
            Temp = Deck; //if temp is set, then leave it.
        }
    }
}