namespace Spades4Player.Core.Logic;
[SingletonGame]
public class SpadesTrickAreaCP(TrickGameContainer<Spades4PlayerCardInformation, Spades4PlayerPlayerItem, Spades4PlayerSaveInfo, EnumSuitList> gameContainer) : SeveralPlayersTrickObservable<EnumSuitList, Spades4PlayerCardInformation, Spades4PlayerPlayerItem, Spades4PlayerSaveInfo>(gameContainer)
{
}