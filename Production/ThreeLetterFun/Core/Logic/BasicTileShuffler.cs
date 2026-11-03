
namespace ThreeLetterFun.Core.Logic;
[SingletonGame]
[AutoReset]
public class BasicTileShuffler(GlobalHelpers global,
    IListShuffler<ThreeLetterFunCardData> deck,
    GameBoard board,
    ThreeLetterFunVMData model
        ) : IShuffleTiles
{
    public async Task StartShufflingAsync(ThreeLetterFunMainGameClass mainGame, int cardsToPassOut = 0)
    {
        mainGame.SaveRoot.CanStart = true;
        deck.ClearObjects();
        deck.ShuffleObjects();
        mainGame.SaveRoot.TileList = global.GetTiles();
        mainGame.SaveRoot.TileList.ShuffleList();
        mainGame.SaveRoot.UpTo = 1;
        DeckRegularDict<ThreeLetterFunCardData> cardList = deck.Take(36).ToRegularDeckDict();
        if (mainGame.SaveRoot.TileList.Count != 100)
        {
            throw new CustomBasicException("Must have 100 tiles");
        }
        if (cardList.Count != 36)
        {
            throw new CustomBasicException("Must have 36 cards");
        }
        if (mainGame.BasicData.MultiPlayer == true)
        {
            mainGame.PlayerList!.ForEach(thisPlayer =>
            {
                thisPlayer.ClearTurn();
                thisPlayer.CardsWon = 0;
                thisPlayer.CardUsed = 0;
                thisPlayer.MostRecent = 0;
                thisPlayer.TookTurn = false;
            });
        }
        DeckRegularDict<ThreeLetterFunCardData> firstList = deck.ToRegularDeckDict();
        if (cardsToPassOut > 0)
        {
            DeckRegularDict<ThreeLetterFunCardData> temps = [];
            ps1.CardProcedures.PassOutCards(mainGame.PlayerList!, firstList, cardsToPassOut, 0, false, ref temps);
        }
        if (mainGame.SaveRoot.Level != EnumLevel.Easy)
        {
            board.ClearBoard(cardList);
        }
        else
        {
            mainGame.SingleInfo = mainGame.PlayerList.GetSelf(); //try this.
            board.ClearBoard(mainGame.SingleInfo!.MainHandList);
        }
        board.Visible = true;
        if (mainGame.BasicData.MultiPlayer == false)
        {
            model.TileBoard1!.UpdateBoard();
            return;
        }
        await mainGame.PopulateSaveRootAsync();
        await mainGame.Network!.SendAllAsync("restoregame", mainGame.SaveRoot); //they need to know what to do now.
        model.TileBoard1!.UpdateBoard(); //try to have no update.  since we have continue turn anyways.
        await mainGame.ContinueTurnAsync();
    }
}