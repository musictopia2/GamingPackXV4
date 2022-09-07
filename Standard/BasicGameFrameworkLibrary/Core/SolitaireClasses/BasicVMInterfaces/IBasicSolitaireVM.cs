namespace BasicGameFrameworkLibrary.Core.SolitaireClasses.BasicVMInterfaces;
public interface IBasicSolitaireVM
{
    DeckObservablePile<SolitaireCard> DeckPile { get; set; }
    SingleObservablePile<SolitaireCard> MainDiscardPile { get; set; }
    IWaste WastePiles1 { get; set; }
    IMain MainPiles1 { get; set; }
    bool CanStartNewGameImmediately { get; set; }
}