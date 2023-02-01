namespace Kismet.Core.Data;
public class KismetDice : SimpleDice
{
    public override void Populate(int chosen)
    {
        base.Populate(chosen);
        if (chosen == 1 || chosen == 6)
        {
            DotColor = cs1.Black;
        }
        else if (chosen == 2 || chosen == 5)
        {
            DotColor = cs1.DarkOrange;
        }
        else
        {
            DotColor = cs1.DarkGreen;
        }
    }
}