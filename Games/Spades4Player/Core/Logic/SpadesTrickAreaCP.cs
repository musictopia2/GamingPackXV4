namespace Spades4Player.Core.Logic;
[SingletonGame]
public class SpadesTrickAreaCP : SeveralPlayersTrickObservable<EnumSuitList, Spades4PlayerCardInformation, Spades4PlayerPlayerItem, Spades4PlayerSaveInfo>
{
    public SpadesTrickAreaCP(TrickGameContainer<Spades4PlayerCardInformation, Spades4PlayerPlayerItem, Spades4PlayerSaveInfo, EnumSuitList> gameContainer) : base(gameContainer)
    {
    }
}