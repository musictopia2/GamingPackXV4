namespace Countdown.Core.Data;
public class CountdownDice : SimpleDice
{
    public override void Populate(int chosen)
    {
        base.Populate(chosen);
        DotColor = cs1.Red;
        FillColor = cs1.White;
    }
}