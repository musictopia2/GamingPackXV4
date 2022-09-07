namespace BasicGameFrameworkLibrary.Core.MultiplayerClasses.InterfaceModels;
public interface ITrickCardGamesData<D, TS> : IBasicCardGamesData<D>
    where TS : IFastEnumSimple
    where D : class, ITrickCard<TS>, new()
{
    TS TrumpSuit { get; set; }
    BasicTrickAreaObservable<TS, D> TrickArea1 { get; set; }
}