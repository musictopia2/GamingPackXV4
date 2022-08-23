namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.PileObservable;
public class CustomMultiplePile : BasicMultiplePilesCP<SolitaireCard>
{
    protected override bool CanAutoUnselect()
    {
        return false;
    }
    public CustomMultiplePile(CommandContainer command) : base(command) { }
}