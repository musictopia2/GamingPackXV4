namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.PileObservable;
public class CustomMultiplePile(CommandContainer command) : BasicMultiplePilesCP<SolitaireCard>(command)
{
    protected override bool CanAutoUnselect()
    {
        return false;
    }
}