namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceModels;

public interface IBasicCardGamesData<D> : IViewModelData where D : IDeckObject, new()
{
    DeckObservablePile<D> Deck1 { get; set; }
    SingleObservablePile<D> Pile1 { get; set; }
    HandObservable<D> PlayerHand1 { get; set; }
    SingleObservablePile<D>? OtherPile { get; set; } //needs to be here to stop the overflows.
}