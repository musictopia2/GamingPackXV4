namespace Risk.Core.Data;
public class AttackDice : SimpleDice
{
    public override void Populate(int Chosen)
    {
        base.Populate(Chosen);
        DotColor = cc1.White;
        FillColor = cc1.Red;
    }
}