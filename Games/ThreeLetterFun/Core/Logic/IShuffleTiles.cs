namespace ThreeLetterFun.Core.Logic;
public interface IShuffleTiles
{
    Task StartShufflingAsync(ThreeLetterFunMainGameClass mainGame, int cardsToPassOut = 0);
}